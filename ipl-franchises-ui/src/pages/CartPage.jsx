import { ArrowLeft, Minus, Plus, ShoppingBag, Trash2 } from 'lucide-react'

import { useState } from 'react'

import { useNavigate } from 'react-router-dom'

import Header from '../components/Header'

import { useCart } from '../context/CartContext'

import { getProductImage } from '../utils/productImages'

import { useOrders } from '../context/OrderContext'

function CartPage() {
  const navigate = useNavigate()

  const {
    cartItems,
    cartCount,
    cartTotal,
    loading,
    error,
    updateQuantity,
    removeFromCart,
    loadCart,
  } = useCart()

  const [busyItemId, setBusyItemId] = useState(null)

  const [clearingCart, setClearingCart] = useState(false)

  const [actionError, setActionError] = useState('')

  const { checkoutOrder, checkoutLoading } = useOrders()

  const handleQuantityChange = async (item, newQuantity) => {
    if (newQuantity < 1 || newQuantity > item.stockQuantity) {
      return
    }

    try {
      setActionError('')

      setBusyItemId(item.id)

      await updateQuantity(item.id, newQuantity)
    } catch (err) {
      setActionError(err.message || 'Unable to update quantity.')
    } finally {
      setBusyItemId(null)
    }
  }

  const handleRemove = async item => {
    try {
      setActionError('')

      setBusyItemId(item.id)

      await removeFromCart(item.id)
    } catch (err) {
      setActionError(err.message || 'Unable to remove item.')
    } finally {
      setBusyItemId(null)
    }
  }

  const handleClearCart = async () => {
    if (cartItems.length === 0) {
      return
    }

    try {
      setActionError('')

      setClearingCart(true)

      /*
       * We currently do not have
       * DELETE /api/cart endpoint.
       *
       * Remove each database cart
       * item individually.
       */

      for (const item of cartItems) {
        await removeFromCart(item.id)
      }
    } catch (err) {
      setActionError(err.message || 'Unable to clear cart.')
    } finally {
      setClearingCart(false)
    }
  }

  const handlePlaceOrder = async () => {
    if (cartItems.length === 0) {
      return
    }

    try {
      setActionError('')

      const order = await checkoutOrder()

      /*
       * Backend checkout clears
       * CartItems in SQL.
       *
       * Reload React cart so the
       * header/cart immediately
       * becomes zero.
       */

      await loadCart()

      const placedId = order?.orderNumber || order?.id

      navigate(`/orders?placed=${encodeURIComponent(placedId)}`)
    } catch (err) {
      if (err.message === 'LOGIN_REQUIRED') {
        navigate('/login?returnUrl=/cart')

        return
      }

      setActionError(err.message || 'Unable to place your order.')
    }
  }

  if (loading) {
    return (
      <>
        <Header />

        <main className="cart-page">
          <div className="cart-container">
            <div className="products-status">Loading your cart...</div>
          </div>
        </main>
      </>
    )
  }

  return (
    <>
      <Header />

      <main className="cart-page">
        <div className="cart-container">
          <button type="button" className="back-to-shop" onClick={() => navigate('/')}>
            <ArrowLeft size={17} />
            Continue Shopping
          </button>

          <div className="cart-title-row">
            <div>
              <h1>Your Cart</h1>

              <p>
                {cartCount} {cartCount === 1 ? 'item' : 'items'} in your cart
              </p>
            </div>

            {cartItems.length > 0 && (
              <button
                type="button"
                className="clear-cart-button"
                disabled={clearingCart}
                onClick={handleClearCart}
              >
                {clearingCart ? 'Clearing...' : 'Clear Cart'}
              </button>
            )}
          </div>

          {(error || actionError) && (
            <div className="cart-error-message">{actionError || error}</div>
          )}

          {cartItems.length === 0 ? (
            <section className="empty-cart">
              <div className="empty-cart-icon">
                <ShoppingBag size={38} />
              </div>

              <h2>Your cart is empty</h2>

              <p>Explore merchandise from your favorite franchises.</p>

              <button
                type="button"
                className="empty-cart-shop-button"
                onClick={() => navigate('/')}
              >
                Start Shopping
              </button>
            </section>
          ) : (
            <div className="cart-layout">
              <section className="cart-items">
                {cartItems.map(item => {
                  const isBusy = busyItemId === item.id

                  const unitPrice = Number(item.unitPrice)

                  const lineTotal =
                    item.subTotal != null ? Number(item.subTotal) : unitPrice * item.quantity

                  return (
                    <article key={item.id} className="cart-item">
                      <div className="cart-item-image-wrap">
                        <img
                          src={getProductImage(item)}
                          alt={item.productName}
                          className="cart-item-image"
                          onError={event => {
                            event.currentTarget.onerror = null

                            event.currentTarget.src = '/images/products/product-placeholder.png'
                          }}
                        />
                      </div>

                      <div className="cart-item-content">
                        <div className="cart-item-top">
                          <div>
                            <div className="cart-item-meta">
                              {item.franchiseCode}

                              {' · '}

                              {item.productType}
                            </div>

                            <h3>{item.productName}</h3>
                            {item.selectedSize && (
                              <div className="cart-item-size">
                                Size: <strong>{item.selectedSize}</strong>
                              </div>
                            )}
                          </div>

                          <button
                            type="button"
                            className="remove-cart-item"
                            disabled={isBusy}
                            onClick={() => handleRemove(item)}
                            aria-label={`Remove ${item.productName}`}
                          >
                            <Trash2 size={17} />
                          </button>
                        </div>

                        <div className="cart-item-bottom">
                          <div className="cart-quantity-control">
                            <button
                              type="button"
                              disabled={isBusy || item.quantity <= 1}
                              onClick={() => handleQuantityChange(item, item.quantity - 1)}
                              aria-label={`Decrease quantity of ${item.productName}`}
                            >
                              <Minus size={14} />
                            </button>

                            <strong>{item.quantity}</strong>

                            <button
                              type="button"
                              disabled={isBusy || item.quantity >= item.stockQuantity}
                              onClick={() => handleQuantityChange(item, item.quantity + 1)}
                              aria-label={`Increase quantity of ${item.productName}`}
                            >
                              <Plus size={14} />
                            </button>
                          </div>

                          <div className="cart-item-price">
                            <span>
                              ₹{unitPrice.toLocaleString('en-IN')}
                              {' × '}
                              {item.quantity}
                            </span>

                            <strong>₹{lineTotal.toLocaleString('en-IN')}</strong>
                          </div>
                        </div>

                        {item.stockQuantity <= 5 && (
                          <div className="cart-low-stock">
                            Only {item.stockQuantity} left in stock
                          </div>
                        )}
                      </div>
                    </article>
                  )
                })}
              </section>

              <aside className="cart-summary">
                <h2>Order Summary</h2>

                <div className="summary-row">
                  <span>Items</span>

                  <span>{cartCount}</span>
                </div>

                <div className="summary-row">
                  <span>Subtotal</span>

                  <strong>₹{Number(cartTotal).toLocaleString('en-IN')}</strong>
                </div>

                <div className="summary-row">
                  <span>Shipping</span>

                  <span className="free-shipping">Free</span>
                </div>

                <div className="summary-divider" />

                <div className="summary-total">
                  <span>Total</span>

                  <strong>₹{Number(cartTotal).toLocaleString('en-IN')}</strong>
                </div>

                <button
                  type="button"
                  className="place-order-button"
                  onClick={handlePlaceOrder}
                  disabled={cartItems.length === 0 || checkoutLoading}
                >
                  {checkoutLoading ? 'Placing Order...' : 'Place Order'}
                </button>

                <p className="cart-summary-note">
                  Prices and stock will be verified securely during checkout.
                </p>
              </aside>
            </div>
          )}
        </div>
      </main>
    </>
  )
}

export default CartPage
