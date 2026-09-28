import { SearchX } from 'lucide-react'

import { useEffect, useMemo, useState } from 'react'

import { useSearchParams } from 'react-router-dom'

import Header from '../components/Header'

import Hero from '../components/Hero'

import CategoryStrip from '../components/CategoryStrip'

import FranchiseSelector from '../components/FranchiseSelector'

import ProductCard from '../components/ProductCard'

import { getProducts } from '../api/productApi'

const productTypes = [
  {
    label: 'All',
    value: '',
  },
  {
    label: 'Jerseys',
    value: 'Jersey',
  },
  {
    label: 'Caps',
    value: 'Cap',
  },
  {
    label: 'Flags',
    value: 'Flag',
  },
  {
    label: 'Autographed',
    value: 'AutographedPhoto',
  },
]

function ProductsPage() {
  const [searchParams, setSearchParams] = useSearchParams()

  /*
   * ===================================
   * URL FILTERS
   * ===================================
   *
   * Examples:
   *
   * /
   *
   * /?type=Jersey
   *
   * /?franchise=CSK
   *
   * /?franchise=RR&type=Cap
   *
   * /?search=CSK
   */

  const searchTerm = searchParams.get('search') || ''

  const selectedFranchise = searchParams.get('franchise') || ''

  const selectedType = searchParams.get('type') || ''

  const [products, setProducts] = useState([])

  const [totalCount, setTotalCount] = useState(0)

  const [loading, setLoading] = useState(true)

  const [error, setError] = useState('')

  const [sortBy, setSortBy] = useState('featured')

  const [refreshKey, setRefreshKey] = useState(0)

  /*
   * Small debounce for search.
   *
   * Avoids calling the API on every
   * single keystroke immediately.
   */

  const [debouncedSearch, setDebouncedSearch] = useState(searchTerm)

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchTerm)
    }, 250)

    return () => clearTimeout(timer)
  }, [searchTerm])

  /*
   * ===================================
   * LOAD PRODUCTS FROM SQL/API
   * ===================================
   */

  useEffect(() => {
    const loadProducts = async () => {
      try {
        setLoading(true)

        setError('')

        const response = await getProducts({
          Search: debouncedSearch,

          Franchise: selectedFranchise,

          Type: selectedType,

          PageNumber: 1,

          PageSize: 50,
        })

        /*
         * API currently returns:
         *
         * {
         *   items: [...],
         *   totalCount: 40,
         *   page: 1,
         *   pageSize: 50,
         *   totalPages: 1
         * }
         */

        const items = Array.isArray(response) ? response : response?.items || []

        setProducts(items)

        setTotalCount(response?.totalCount ?? items.length)
      } catch (err) {
        console.error('Unable to load products', err)

        setProducts([])

        setTotalCount(0)

        setError(err.message || 'Unable to load merchandise.')
      } finally {
        setLoading(false)
      }
    }

    loadProducts()
  }, [debouncedSearch, selectedFranchise, selectedType, refreshKey])

  /*
   * ===================================
   * HEADER SEARCH
   * ===================================
   *
   * Search combines with the currently
   * selected team/type.
   *
   * Example:
   *
   * franchise=RR + search=CSK
   *
   * returns 0 because filters use
   * cumulative AND logic.
   */

  const handleSearch = value => {
    const next = new URLSearchParams(searchParams)

    const cleanedValue = value.trim()

    if (cleanedValue) {
      next.set('search', cleanedValue)
    } else {
      next.delete('search')
    }

    setSearchParams(next, {
      replace: true,
    })
  }

  /*
   * ===================================
   * TEAM FILTER
   * ===================================
   */

  const handleFranchiseSelect = franchiseCode => {
    const next = new URLSearchParams(searchParams)

    /*
     * Clicking selected team again
     * clears that team filter.
     */

    if (selectedFranchise === franchiseCode) {
      next.delete('franchise')
    } else {
      next.set('franchise', franchiseCode)
    }

    setSearchParams(next)

    /*
     * Move user smoothly toward
     * merchandise after choosing
     * team.
     */

    setTimeout(() => {
      const element = document.getElementById('merchandise')

      if (!element) {
        return
      }

      const headerOffset = 145

      const position = element.getBoundingClientRect().top + window.scrollY

      window.scrollTo({
        top: position - headerOffset,

        behavior: 'smooth',
      })
    }, 50)
  }

  /*
   * ===================================
   * TYPE FILTER PILLS
   * ===================================
   */

  const handleTypeSelect = type => {
    const next = new URLSearchParams(searchParams)

    if (type) {
      next.set('type', type)
    } else {
      next.delete('type')
    }

    setSearchParams(next)
  }

  /*
   * ===================================
   * REMOVE INDIVIDUAL FILTERS
   * ===================================
   */

  const clearFranchise = () => {
    const next = new URLSearchParams(searchParams)

    next.delete('franchise')

    setSearchParams(next)
  }

  const clearType = () => {
    const next = new URLSearchParams(searchParams)

    next.delete('type')

    setSearchParams(next)
  }

  const clearSearch = () => {
    const next = new URLSearchParams(searchParams)

    next.delete('search')

    setSearchParams(next)
  }

  const clearAllFilters = () => {
    setSearchParams({})
  }

  /*
   * ===================================
   * SORT PRODUCTS CLIENT-SIDE
   * ===================================
   */

  const sortedProducts = useMemo(() => {
    const result = [...products]

    switch (sortBy) {
      case 'price-low':
        return result.sort((a, b) => Number(a.price) - Number(b.price))

      case 'price-high':
        return result.sort((a, b) => Number(b.price) - Number(a.price))

      case 'name':
        return result.sort((a, b) => String(a.name).localeCompare(String(b.name)))

      default:
        /*
         * API order = Featured
         */
        return result
    }
  }, [products, sortBy])

  /*
   * ===================================
   * HEADING
   * ===================================
   */

  const merchandiseTitle = useMemo(() => {
    if (selectedFranchise && selectedType) {
      const typeLabel = productTypes.find(type => type.value === selectedType)?.label

      return `${selectedFranchise} ${typeLabel || 'Merchandise'}`
    }

    if (selectedFranchise) {
      return `${selectedFranchise} Merchandise`
    }

    if (selectedType) {
      return productTypes.find(type => type.value === selectedType)?.label || 'Merchandise'
    }

    if (searchTerm) {
      return 'Search Results'
    }

    return 'Featured Merchandise'
  }, [selectedFranchise, selectedType, searchTerm])

  const hasActiveFilters = Boolean(selectedFranchise || selectedType || searchTerm)

  return (
    <>
      <Header onSearch={handleSearch} />

      {/* HERO */}

      <Hero />

      {/* QUICK CATEGORY CARDS */}

      <CategoryStrip />

      {/* ===================================
          SHOP BY TEAM
      =================================== */}

      <section id="teams" className="teams-section">
        <div className="section-heading-row">
          <div>
            <span className="section-eyebrow">CHOOSE YOUR SIDE</span>

            <h2>Shop by Team</h2>

            <p>Explore merchandise from your favorite franchise.</p>
          </div>
        </div>

        <FranchiseSelector
          selectedFranchise={selectedFranchise}
          onSelectFranchise={handleFranchiseSelect}
        />
      </section>

      {/* ===================================
          MERCHANDISE
      =================================== */}

      <main id="merchandise" className="products-section">
        <div className="products-heading-row">
          <div>
            <span className="section-eyebrow">FAN STORE</span>

            <h2>{merchandiseTitle}</h2>

            {!loading && (
              <p className="products-result-count">
                {totalCount} {totalCount === 1 ? 'product' : 'products'}
              </p>
            )}
          </div>

          <div className="products-sort">
            <label htmlFor="product-sort">Sort</label>

            <select
              id="product-sort"
              value={sortBy}
              onChange={event => setSortBy(event.target.value)}
            >
              <option value="featured">Featured</option>

              <option value="price-low">Price: Low to High</option>

              <option value="price-high">Price: High to Low</option>

              <option value="name">Name: A-Z</option>
            </select>
          </div>
        </div>

        {/* PRODUCT TYPE FILTERS */}

        <div className="products-toolbar">
          <div className="type-filters">
            {productTypes.map(type => (
              <button
                key={type.label}
                type="button"
                className={selectedType === type.value ? 'active' : ''}
                onClick={() => handleTypeSelect(type.value)}
              >
                {type.label}
              </button>
            ))}
          </div>
        </div>

        {/* ACTIVE FILTERS */}

        {hasActiveFilters && (
          <div className="active-filters">
            <span className="active-filters-label">Filters:</span>

            {selectedFranchise && (
              <button type="button" className="filter-chip" onClick={clearFranchise}>
                Team: {selectedFranchise}
                <span>×</span>
              </button>
            )}

            {selectedType && (
              <button type="button" className="filter-chip" onClick={clearType}>
                {productTypes.find(type => type.value === selectedType)?.label}

                <span>×</span>
              </button>
            )}

            {searchTerm && (
              <button type="button" className="filter-chip" onClick={clearSearch}>
                Search:
                {' "'}
                {searchTerm}
                {'"'}
                <span>×</span>
              </button>
            )}

            <button type="button" className="clear-all-filters" onClick={clearAllFilters}>
              Clear All
            </button>
          </div>
        )}

        {/* LOADING */}

        {loading && <div className="products-status">Loading merchandise...</div>}

        {/* ERROR */}

        {!loading && error && (
          <div className="products-status error-message">
            <strong>Unable to load merchandise</strong>

            <span>{error}</span>

            <button type="button" onClick={() => setRefreshKey(value => value + 1)}>
              Try Again
            </button>
          </div>
        )}

        {/* EMPTY RESULT */}

        {!loading && !error && sortedProducts.length === 0 && (
          <section className="products-empty-state">
            <SearchX size={34} />

            <h3>No merchandise found</h3>

            <p>
              {hasActiveFilters
                ? 'No products match the selected filters. Try removing one or more filters.'
                : 'No products are currently available.'}
            </p>

            {hasActiveFilters && (
              <button type="button" onClick={clearAllFilters}>
                Clear All Filters
              </button>
            )}
          </section>
        )}

        {/* PRODUCT GRID */}

        {!loading && !error && sortedProducts.length > 0 && (
          <div className="products-grid">
            {sortedProducts.map(product => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )}
      </main>
    </>
  )
}

export default ProductsPage
