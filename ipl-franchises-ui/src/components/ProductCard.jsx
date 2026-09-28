import { ArrowRight, Heart, ShoppingCart } from 'lucide-react'

import { useState } from 'react'

import { useLocation, useNavigate } from 'react-router-dom'

import { useCart } from '../context/CartContext'

import { getProductImage } from '../utils/productImages'

const teamClass = code => `product-team-${String(code || '').toLowerCase()}`

function ProductCard({ product }) {
  const navigate = useNavigate()

  const location = useLocation()

  const { addToCart } = useCart()

  const [liked, setLiked] = useState(false)

  const [added, setAdded] = useState(false)

  const [adding, setAdding] = useState(false)

  const isLowStock = product.stockQuantity > 0 && product.stockQuantity <= 10

  const isJersey =
    String(product.productType || '')
      .trim()
      .toLowerCase() === 'jersey'

  const openProduct = () => {
    navigate(`/products/${product.id}`)
  }

  const handleAddToCart = async event => {
    event.stopPropagation()

    if (product.stockQuantity <= 0) {
      return
    }

    /*
     * Jerseys need a size.
     *
     * Send shopper to Product
     * Details to choose:
     *
     * M / L / XL
     */

    if (isJersey) {
      openProduct()

      return
    }

    try {
      setAdding(true)

      await addToCart(product, 1, null)

      setAdded(true)

      setTimeout(() => {
        setAdded(false)
      }, 1200)
    } catch (err) {
      if (err.message === 'LOGIN_REQUIRED') {
        const returnUrl = location.pathname

        navigate(`/login?returnUrl=${encodeURIComponent(returnUrl)}`)

        return
      }

      console.error('Unable to add product', err)
    } finally {
      setAdding(false)
    }
  }

  return (
    <article className="product-card">
      <div
        className={`
          product-visual
          ${teamClass(product.franchiseCode)}
        `}
      >
        <span className="product-team-chip">{product.franchiseCode}</span>

        <button
          className={`
            favorite-button
            ${liked ? 'liked' : ''}
          `}
          type="button"
          onClick={event => {
            event.stopPropagation()

            setLiked(value => !value)
          }}
          aria-label={
            liked ? `Remove ${product.name} from favorites` : `Add ${product.name} to favorites`
          }
        >
          <Heart size={17} fill={liked ? 'currentColor' : 'none'} />
        </button>

        <img
          src={getProductImage(product)}
          alt={product.name}
          className="product-image"
          onClick={openProduct}
          onError={event => {
            event.currentTarget.onerror = null

            event.currentTarget.src = '/images/products/product-placeholder.png'
          }}
        />
      </div>

      <div className="product-info">
        <div className="product-meta">
          {product.franchiseCode}

          {' · '}

          {product.productType}
        </div>

        <h3 className="product-title-link" onClick={openProduct}>
          {product.name}
        </h3>

        <div className="product-price-row">
          <strong>₹{Number(product.price).toLocaleString('en-IN')}</strong>

          {product.stockQuantity === 0 ? (
            <span className="stock-pill out">Out of stock</span>
          ) : isLowStock ? (
            <span className="stock-pill low">Only {product.stockQuantity} left</span>
          ) : (
            <span className="stock-pill">In stock</span>
          )}
        </div>

        <div className="product-actions">
          <button
            className={`
              add-cart-button
              ${added ? 'added' : ''}
            `}
            type="button"
            disabled={product.stockQuantity === 0 || adding}
            onClick={handleAddToCart}
          >
            <ShoppingCart size={16} />

            {adding ? 'Adding...' : added ? 'Added!' : isJersey ? 'Select Size' : 'Add to Cart'}
          </button>

          <button
            className="details-button"
            type="button"
            onClick={openProduct}
            aria-label={`View ${product.name}`}
          >
            <ArrowRight size={18} />
          </button>
        </div>
      </div>
    </article>
  )
}

export default ProductCard
