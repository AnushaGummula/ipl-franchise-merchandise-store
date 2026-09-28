import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'

import { checkout, getOrders } from '../api/orderApi'

import { useAuth } from './AuthContext'

const OrderContext = createContext(null)

export function OrderProvider({ children }) {
  const { token, isAuthenticated, loading: authLoading } = useAuth()

  const [orders, setOrders] = useState([])

  const [loading, setLoading] = useState(false)

  const [checkoutLoading, setCheckoutLoading] = useState(false)

  const [error, setError] = useState('')

  const resetOrders = useCallback(() => {
    setOrders([])
    setError('')
  }, [])

  const loadOrders = useCallback(async () => {
    if (!isAuthenticated || !token) {
      resetOrders()

      return
    }

    try {
      setLoading(true)

      setError('')

      const result = await getOrders(token)

      setOrders(Array.isArray(result) ? result : [])
    } catch (err) {
      setError(err.message || 'Unable to load orders.')
    } finally {
      setLoading(false)
    }
  }, [isAuthenticated, token, resetOrders])

  useEffect(() => {
    if (authLoading) {
      return
    }

    loadOrders()
  }, [authLoading, loadOrders])

  const checkoutOrder = async () => {
    if (!isAuthenticated || !token) {
      throw new Error('LOGIN_REQUIRED')
    }

    try {
      setCheckoutLoading(true)

      setError('')

      const order = await checkout(token)

      await loadOrders()

      return order
    } catch (err) {
      setError(err.message || 'Unable to place order.')

      throw err
    } finally {
      setCheckoutLoading(false)
    }
  }

  const orderCount = useMemo(() => orders.length, [orders])

  return (
    <OrderContext.Provider
      value={{
        orders,
        orderCount,

        loading,
        checkoutLoading,
        error,

        checkoutOrder,
        loadOrders,
        resetOrders,
      }}
    >
      {children}
    </OrderContext.Provider>
  )
}

export function useOrders() {
  const context = useContext(OrderContext)

  if (!context) {
    throw new Error('useOrders must be used inside OrderProvider')
  }

  return context
}
