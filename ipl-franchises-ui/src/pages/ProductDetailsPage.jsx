import { ArrowLeft, Minus, Plus, ShoppingCart } from 'lucide-react'

import { useEffect, useState } from 'react'

import { useNavigate, useParams } from 'react-router-dom'

import Header from '../components/Header'

import { useCart } from '../context/CartContext'

import { getProductById } from '../api/productApi'

import { getProductImage } from '../utils/productImages'

function ProductDetailsPage() {
  const { productId } = useParams()

  const navigate = useNavigate()

  const { addToCart } = useCart()

  const [product, setProduct] = useState(null)

  const [loading, setLoading] = useState(true)

  const [error, setError] = useState('')

  const [quantity, setQuantity] = useState(1)

  const [selectedSize, setSelectedSize] = useState('')

  const [addingToCart, setAddingToCart] = useState(false)

  const [added, setAdded] = useState(false)

  const [cartError, setCartError] = useState('')

  useEffect(() => {
    const loadProduct = async () => {
      try {
        setLoading(true)

        setError('')

        const result = await getProductById(productId)

        setProduct(result)
      } catch (err) {
        console.error('Unable to load product', err)

        if (err.status === 404) {
          setError('Product could not be found.')
        } else {
          setError(err.message || 'Product could not be loaded.')
        }
      } finally {
        setLoading(false)
      }
    }

    loadProduct()
  }, [productId])

  const isJersey =
    String(product?.productType || '')
      .trim()
      .toLowerCase() === 'jersey'

  const increaseQuantity = () => {
    if (!product) {
      return
    }

    if (quantity < product.stockQuantity) {
      setQuantity(current => current + 1)
    }
  }

  const decreaseQuantity = () => {
    setQuantity(current => Math.max(1, current - 1))
  }

  const handleSizeSelect = size => {
    setSelectedSize(size)

    setCartError('')
  }

  const handleAddToCart = async () => {
    if (!product) {
      return
    }

    /*
     * Jerseys must have
     * M / L / XL selected.
     */

    if (isJersey && !selectedSize) {
      setCartError('Please select a jersey size.')

      return
    }

    if (product.stockQuantity <= 0) {
      setCartError('This product is currently out of stock.')

      return
    }

    try {
      setAddingToCart(true)

      setCartError('')

      setAdded(false)

      await addToCart(product, quantity, isJersey ? selectedSize : null)

      setAdded(true)

      setTimeout(() => {
        setAdded(false)
      }, 1500)
    } catch (err) {
      /*
       * CartContext throws
       * LOGIN_REQUIRED when
       * user is not logged in.
       */

      if (err.message === 'LOGIN_REQUIRED') {
        const returnUrl = `/products/${product.id}`

        navigate(`/login?returnUrl=${encodeURIComponent(returnUrl)}`)

        return
      }

      setCartError(err.message || 'Unable to add this product to your cart.')
    } finally {
      setAddingToCart(false)
    }
  }

  return (
    <>
      <Header />

      <main className="product-details-page">
        <div className="product-details-container">
          <button type="button" className="back-to-shop" onClick={() => navigate('/')}>
            <ArrowLeft size={17} />
            Back to Shop
          </button>

          {loading && <div className="products-status">Loading product...</div>}

          {!loading && error && (
            <div className="products-status error-message">
              <strong>Product unavailable</strong>

              <span>{error}</span>
            </div>
          )}

          {!loading && !error && product && (
            <section className="product-details-layout">
              {/* PRODUCT IMAGE */}

              <div className="product-details-image-panel">
                <img
                  src={getProductImage(product)}
                  alt={product.name}
                  className="product-details-image"
                  onError={event => {
                    event.currentTarget.onerror = null

                    event.currentTarget.src = '/images/products/product-placeholder.png'
                  }}
                />
              </div>

              {/* PRODUCT INFORMATION */}

              <div className="product-details-content">
                <div className="product-details-meta">
                  {product.franchiseCode}

                  <span>•</span>

                  {product.productType}
                </div>

                <h1>{product.name}</h1>

                <div className="product-details-price">
                  ₹{Number(product.price).toLocaleString('en-IN')}
                </div>

                {/* STOCK */}

                {product.stockQuantity > 0 ? (
                  <div className="details-stock available">In stock</div>
                ) : (
                  <div className="details-stock unavailable">Out of stock</div>
                )}

                <div className="details-divider" />

                {/* DESCRIPTION */}

                <div className="details-description">
                  <h3>Product Details</h3>

                  <p>
                    Show your support for {product.franchiseName || product.franchiseCode} with this{' '}
                    {String(product.productType)
                      .replace('AutographedPhoto', 'autographed photo')
                      .toLowerCase()}
                    .
                  </p>

                  <p>
                    Designed for cricket fans looking for quality franchise merchandise and
                    match-day essentials.
                  </p>
                </div>

                {product.stockQuantity > 0 && (
                  <>
                    {/* SIZE - ONLY JERSEYS */}

                    {isJersey && (
                      <div className="product-size-section">
                        <div className="size-heading-row">
                          <span className="size-label">Select Size</span>

                          {selectedSize && (
                            <span className="selected-size-text">
                              Selected: <strong>{selectedSize}</strong>
                            </span>
                          )}
                        </div>

                        <div className="size-options">
                          {['M', 'L', 'XL'].map(size => (
                            <button
                              key={size}
                              type="button"
                              className={
                                selectedSize === size ? 'size-option active' : 'size-option'
                              }
                              onClick={() => handleSizeSelect(size)}
                            >
                              {size}
                            </button>
                          ))}
                        </div>
                      </div>
                    )}

                    {/* CART ERROR */}

                    {cartError && <div className="product-cart-error">{cartError}</div>}

                    {/* QUANTITY */}

                    <div className="quantity-section">
                      <span>Quantity</span>

                      <div className="quantity-control">
                        <button
                          type="button"
                          onClick={decreaseQuantity}
                          disabled={quantity <= 1 || addingToCart}
                          aria-label="Decrease quantity"
                        >
                          <Minus size={16} />
                        </button>

                        <strong>{quantity}</strong>

                        <button
                          type="button"
                          onClick={increaseQuantity}
                          disabled={quantity >= product.stockQuantity || addingToCart}
                          aria-label="Increase quantity"
                        >
                          <Plus size={16} />
                        </button>
                      </div>
                    </div>

                    {/* ADD TO CART */}

                    <button
                      type="button"
                      className={`
                        details-add-cart-button
                        ${added ? 'added' : ''}
                      `}
                      disabled={addingToCart}
                      onClick={handleAddToCart}
                    >
                      <ShoppingCart size={19} />

                      {addingToCart
                        ? 'Adding...'
                        : added
                          ? 'Added to Cart!'
                          : `Add ${quantity} to Cart`}
                    </button>

                    {isJersey && !selectedSize && (
                      <div className="size-helper-text">
                        Please choose M, L or XL before adding this jersey to your cart.
                      </div>
                    )}
                  </>
                )}
              </div>
            </section>
          )}
        </div>
      </main>
    </>
  )
}

export default ProductDetailsPage
