import { ArrowLeft, CheckCircle2, Package, ShoppingBag } from 'lucide-react'

import { useEffect, useMemo, useState } from 'react'

import { useNavigate, useSearchParams } from 'react-router-dom'

import Header from '../components/Header'

import ProductCard from '../components/ProductCard'

import { useOrders } from '../context/OrderContext'

import { getProducts } from '../api/productApi'

import { getProductImage } from '../utils/productImages'

import { getRecommendations } from '../utils/productRecommendations'

function OrdersPage() {
  const navigate = useNavigate()

  const [searchParams] = useSearchParams()

  const { orders, loading, error } = useOrders()

  const placedOrderId = searchParams.get('placed')

  const [products, setProducts] = useState([])

  const [recommendationsLoading, setRecommendationsLoading] = useState(false)

  /*
   * LOAD PRODUCTS FOR
   * "YOU MIGHT ALSO LIKE"
   *
   * We only need this immediately
   * after an order was placed.
   */
  useEffect(() => {
    if (!placedOrderId) {
      return
    }

    const loadProducts = async () => {
      try {
        setRecommendationsLoading(true)

        const result = await getProducts({
          PageNumber: 1,
          PageSize: 50,
        })

        /*
         * Supports either:
         *
         * [...]
         *
         * or
         *
         * {
         *   items: [...]
         * }
         *
         * depending on backend
         * pagination response.
         */
        const productList = Array.isArray(result) ? result : result?.items || result?.data || []

        setProducts(productList)
      } catch (err) {
        console.error('Unable to load recommendations', err)

        setProducts([])
      } finally {
        setRecommendationsLoading(false)
      }
    }

    loadProducts()
  }, [placedOrderId])

  /*
   * FIND THE ORDER THAT WAS
   * JUST PLACED.
   *
   * placedOrderId can contain
   * either:
   *
   * orderNumber
   *
   * or
   *
   * numeric order id
   */
  const placedOrder = useMemo(() => {
    if (!placedOrderId || !Array.isArray(orders)) {
      return null
    }

    return (
      orders.find(
        order =>
          String(order.orderNumber) === String(placedOrderId) ||
          String(order.id) === String(placedOrderId)
      ) || null
    )
  }, [orders, placedOrderId])

  /*
   * BUILD MAX 4
   * RECOMMENDATIONS.
   */
  const recommendations = useMemo(() => {
    if (!placedOrder) {
      return []
    }

    return getRecommendations(products, placedOrder.items || [], 4)
  }, [products, placedOrder])

  const formatDate = value => {
    if (!value) {
      return ''
    }

    return new Intl.DateTimeFormat('en-IN', {
      dateStyle: 'medium',

      timeStyle: 'short',
    }).format(new Date(value))
  }

  return (
    <>
      <Header />

      <main className="orders-page">
        <div className="orders-container">
          {/* BACK */}

          <button type="button" className="back-to-shop" onClick={() => navigate('/')}>
            <ArrowLeft size={17} />
            Back to Shop
          </button>

          {/* SUCCESS MESSAGE */}

          {placedOrderId && (
            <div className="order-success-banner">
              <CheckCircle2 size={23} />

              <div>
                <strong>Order placed successfully</strong>

                <span>Order reference: {placedOrderId}</span>
              </div>
            </div>
          )}

          {/* PAGE HEADING */}

          <div className="orders-heading">
            <div>
              <span className="orders-eyebrow">MY ACCOUNT</span>

              <h1>Order History</h1>

              <p>View your previous merchandise purchases.</p>
            </div>

            {orders.length > 0 && (
              <div className="orders-count">
                {orders.length} {orders.length === 1 ? 'order' : 'orders'}
              </div>
            )}
          </div>

          {/* LOADING */}

          {loading && <div className="products-status">Loading your orders...</div>}

          {/* ERROR */}

          {!loading && error && <div className="orders-error">{error}</div>}

          {/* EMPTY */}

          {!loading && !error && orders.length === 0 && (
            <section className="empty-orders">
              <div className="empty-orders-icon">
                <Package size={38} />
              </div>

              <h2>No orders yet</h2>

              <p>Once you place an order, it will appear here.</p>

              <button type="button" onClick={() => navigate('/')}>
                <ShoppingBag size={17} />
                Shop Merchandise
              </button>
            </section>
          )}

          {/* ORDER LIST */}

          {!loading && !error && orders.length > 0 && (
            <div className="orders-list">
              {orders.map(order => (
                <article key={order.id} className="order-card">
                  {/* ORDER HEADER */}

                  <div className="order-card-header">
                    <div className="order-id-section">
                      <span>ORDER</span>

                      <strong>{order.orderNumber || `#${order.id}`}</strong>
                    </div>

                    <div className="order-header-right">
                      <span className="order-date">
                        {formatDate(order.orderDate || order.createdAt)}
                      </span>

                      <span className="order-status">{order.status || 'Placed'}</span>
                    </div>
                  </div>

                  {/* ORDER ITEMS */}

                  <div className="order-items">
                    {order.items?.map((item, index) => (
                      <div
                        key={`${order.id}-${item.productId}-${item.selectedSize || 'NA'}-${index}`}
                        className="order-item"
                      >
                        {/* IMAGE */}

                        <div className="order-item-image-wrap">
                          <img
                            src={getProductImage({
                              franchiseCode: item.franchiseCode,

                              productType: item.productType,
                            })}
                            alt={item.productName}
                            className="order-item-image"
                            onError={event => {
                              event.currentTarget.onerror = null

                              event.currentTarget.src = '/images/products/product-placeholder.png'
                            }}
                          />
                        </div>

                        {/* PRODUCT INFO */}

                        <div className="order-item-info">
                          <span className="order-item-meta">
                            {item.franchiseCode}

                            {item.productType && (
                              <>
                                {' · '}

                                {item.productType}
                              </>
                            )}
                          </span>

                          <strong>{item.productName}</strong>

                          <div className="order-item-submeta">
                            {item.selectedSize && (
                              <span>
                                Size: <b>{item.selectedSize}</b>
                              </span>
                            )}

                            <span>
                              Qty: <b>{item.quantity}</b>
                            </span>
                          </div>
                        </div>

                        {/* PRICE */}

                        <div className="order-item-price">
                          <span>
                            ₹{Number(item.unitPrice).toLocaleString('en-IN')}
                            {' × '}
                            {item.quantity}
                          </span>

                          <strong>
                            ₹
                            {Number(
                              item.subTotal ?? Number(item.unitPrice) * item.quantity
                            ).toLocaleString('en-IN')}
                          </strong>
                        </div>
                      </div>
                    ))}
                  </div>

                  {/* ORDER FOOTER */}

                  <div className="order-card-footer">
                    <span>
                      {order.itemCount ??
                        order.items?.reduce((total, item) => total + item.quantity, 0) ??
                        0}{' '}
                      {(order.itemCount ??
                        order.items?.reduce((total, item) => total + item.quantity, 0) ??
                        0) === 1
                        ? 'item'
                        : 'items'}
                    </span>

                    <div>
                      <span>Order Total</span>

                      <strong>
                        ₹{Number(order.totalAmount ?? order.total ?? 0).toLocaleString('en-IN')}
                      </strong>
                    </div>
                  </div>
                </article>
              ))}
            </div>
          )}

          {/* ===================================
              YOU MIGHT ALSO LIKE
          =================================== */}

          {placedOrderId && !loading && !error && (
            <section className="recommendations-section">
              <div className="recommendations-heading">
                <div>
                  <span>KEEP SHOPPING</span>

                  <h2>You Might Also Like</h2>

                  <p>More merchandise selected from the fan store.</p>
                </div>

                <button type="button" onClick={() => navigate('/')}>
                  View All Products
                </button>
              </div>

              {recommendationsLoading ? (
                <div className="products-status">Loading recommendations...</div>
              ) : recommendations.length > 0 ? (
                <div className="recommendations-grid">
                  {recommendations.map(product => (
                    <ProductCard key={product.id} product={product} />
                  ))}
                </div>
              ) : (
                <div className="recommendations-empty">
                  <Package size={25} />

                  <strong>More merchandise awaits</strong>

                  <span>Browse products from all franchises.</span>

                  <button type="button" onClick={() => navigate('/')}>
                    Continue Shopping
                  </button>
                </div>
              )}
            </section>
          )}
        </div>
      </main>
    </>
  )
}

export default OrdersPage
