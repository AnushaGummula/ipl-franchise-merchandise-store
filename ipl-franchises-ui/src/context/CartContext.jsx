import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'

import { addCartItem, getCart, removeCartItem, updateCartItem } from '../api/cartApi'

import { useAuth } from './AuthContext'

const CartContext = createContext(null)

export function CartProvider({ children }) {
  const { token, isAuthenticated, loading: authLoading } = useAuth()

  const [cart, setCart] = useState({
    items: [],
    totalAmount: 0,
    totalItems: 0,
  })

  const [loading, setLoading] = useState(false)

  const [error, setError] = useState('')

  const resetCart = useCallback(() => {
    setCart({
      items: [],
      totalAmount: 0,
      totalItems: 0,
    })

    setError('')
  }, [])

  const loadCart = useCallback(async () => {
    if (!isAuthenticated || !token) {
      resetCart()

      return
    }

    try {
      setLoading(true)

      setError('')

      const result = await getCart(token)

      setCart(
        result || {
          items: [],
          totalAmount: 0,
          totalItems: 0,
        }
      )
    } catch (err) {
      setError(err.message || 'Unable to load cart.')
    } finally {
      setLoading(false)
    }
  }, [isAuthenticated, token, resetCart])

  useEffect(() => {
    if (authLoading) {
      return
    }

    loadCart()
  }, [authLoading, loadCart])

  const addToCart = async (product, quantity = 1, selectedSize = null) => {
    if (!isAuthenticated || !token) {
      throw new Error('LOGIN_REQUIRED')
    }

    try {
      setError('')

      const result = await addCartItem(token, product.id, quantity, selectedSize)

      setCart(result)

      return result
    } catch (err) {
      setError(err.message || 'Unable to add item.')

      throw err
    }
  }

  const updateQuantity = async (cartItemId, quantity) => {
    if (!token) {
      return
    }

    if (quantity < 1) {
      return
    }

    const result = await updateCartItem(token, cartItemId, quantity)

    setCart(result)

    return result
  }

  const removeFromCart = async cartItemId => {
    if (!token) {
      return
    }

    await removeCartItem(token, cartItemId)

    await loadCart()
  }

  const clearCartState = () => {
    resetCart()
  }

  const cartItems = cart?.items ?? []

  const cartCount = useMemo(
    () => cart.totalItems ?? cartItems.reduce((sum, item) => sum + item.quantity, 0),
    [cart.totalItems, cartItems]
  )

  const cartTotal = useMemo(
    () =>
      Number(
        cart.totalAmount ??
          cartItems.reduce((sum, item) => sum + Number(item.unitPrice) * item.quantity, 0)
      ),
    [cart.totalAmount, cartItems]
  )

  return (
    <CartContext.Provider
      value={{
        cartItems,
        cartCount,
        cartTotal,
        loading,
        error,

        addToCart,
        updateQuantity,
        removeFromCart,
        loadCart,
        clearCartState,
      }}
    >
      {children}
    </CartContext.Provider>
  )
}

export function useCart() {
  const context = useContext(CartContext)

  if (!context) {
    throw new Error('useCart must be used inside CartProvider')
  }

  return context
}
