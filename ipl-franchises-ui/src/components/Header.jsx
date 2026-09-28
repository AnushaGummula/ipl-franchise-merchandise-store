import {
  ChevronDown,
  LogIn,
  LogOut,
  Menu,
  PackageSearch,
  Search,
  ShoppingBag,
  UserRound,
  X,
} from 'lucide-react'

import { useEffect, useRef, useState } from 'react'

import { useLocation, useNavigate } from 'react-router-dom'

import { useAuth } from '../context/AuthContext'

import { useCart } from '../context/CartContext'

function Header({ onSearch }) {
  const navigate = useNavigate()

  const location = useLocation()

  const { user, isAuthenticated, logout } = useAuth()

  const { cartCount } = useCart()

  const [searchValue, setSearchValue] = useState('')

  const [accountOpen, setAccountOpen] = useState(false)

  const [mobileOpen, setMobileOpen] = useState(false)

  const accountRef = useRef(null)

  const shopType = type => {
    setMobileOpen(false)

    navigate(`/?type=${encodeURIComponent(type)}`)

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
    }, 150)
  }

  const firstName = user?.fullName?.trim()?.split(' ')[0] || 'Fan'

  const handleBrandClick = () => {
    setMobileOpen(false)

    /*
     * Clear filters/search query params
     * and return to storefront home.
     */
    navigate('/')

    setTimeout(() => {
      window.scrollTo({
        top: 0,
        behavior: 'smooth',
      })
    }, 50)
  }

  useEffect(() => {
    const handleOutsideClick = event => {
      if (accountRef.current && !accountRef.current.contains(event.target)) {
        setAccountOpen(false)
      }
    }

    document.addEventListener('mousedown', handleOutsideClick)

    return () => {
      document.removeEventListener('mousedown', handleOutsideClick)
    }
  }, [])

  const scrollToSection = sectionId => {
    setMobileOpen(false)

    const scrollToTarget = () => {
      const element = document.getElementById(sectionId)

      if (!element) {
        return
      }

      const headerOffset = 135

      const elementPosition = element.getBoundingClientRect().top + window.scrollY

      window.scrollTo({
        top: elementPosition - headerOffset,

        behavior: 'smooth',
      })
    }

    if (location.pathname !== '/') {
      navigate('/')

      setTimeout(scrollToTarget, 180)

      return
    }

    scrollToTarget()
  }

  const handleSearch = event => {
    const value = event.target.value

    setSearchValue(value)

    if (onSearch) {
      onSearch(value)
    }
  }

  const submitSearch = event => {
    event.preventDefault()

    if (location.pathname !== '/') {
      navigate('/')
    } else {
      document.getElementById('merchandise')?.scrollIntoView({
        behavior: 'smooth',
      })
    }
  }

  const handleLogout = () => {
    logout()

    setAccountOpen(false)

    navigate('/')
  }

  return (
    <header className="store-header">
      {/* TOP UTILITY BAR */}

      <div className="store-topbar">
        <div className="store-topbar-inner">
          <span>FAN STORE DEMO</span>

          <span className="topbar-offer">Free shipping on orders above ₹999</span>

          <span className="topbar-help">Cricket merchandise for every fan</span>
        </div>
      </div>

      {/* MAIN HEADER */}

      <div className="store-main-header">
        <button
          type="button"
          className="mobile-menu-button"
          onClick={() => setMobileOpen(value => !value)}
          aria-label="Open menu"
        >
          {mobileOpen ? <X size={22} /> : <Menu size={22} />}
        </button>

        {/* BRAND */}

        <button
          type="button"
          className="store-brand"
          onClick={handleBrandClick}
          aria-label="Go to IPL Merchandise home"
        >
          <img src="/favicon.svg" alt="" className="store-brand-logo" />

          <span className="store-brand-copy">
            <strong>IPL MERCHANDISE</strong>

            <span>FAN STORE DEMO</span>
          </span>
        </button>

        {/* SEARCH */}

        <form className="store-search" onSubmit={submitSearch}>
          <div className="store-search-category">All</div>

          <input
            type="search"
            value={searchValue}
            placeholder="Search jerseys, caps, flags and merchandise"
            onChange={handleSearch}
            aria-label="Search merchandise"
          />

          <button type="submit" aria-label="Search">
            <Search size={20} />
          </button>
        </form>

        {/* ACCOUNT */}

        <div className="store-account" ref={accountRef}>
          <button
            type="button"
            className="header-action"
            onClick={() => setAccountOpen(value => !value)}
          >
            <UserRound size={22} />

            <div className="header-action-copy">
              <small>{isAuthenticated ? `Hello, ${firstName}` : 'Hello, sign in'}</small>

              <strong>Account</strong>
            </div>

            <ChevronDown size={14} />
          </button>

          {accountOpen && (
            <div className="account-dropdown">
              {isAuthenticated ? (
                <>
                  <div className="account-dropdown-user">
                    <div className="account-avatar">{firstName.charAt(0).toUpperCase()}</div>

                    <div>
                      <strong>{user?.fullName}</strong>

                      <span>{user?.email}</span>
                    </div>
                  </div>

                  <div className="account-dropdown-divider" />

                  <button
                    type="button"
                    onClick={() => {
                      setAccountOpen(false)

                      navigate('/orders')
                    }}
                  >
                    <PackageSearch size={17} />
                    My Orders
                  </button>

                  <button type="button" onClick={handleLogout}>
                    <LogOut size={17} />
                    Sign Out
                  </button>
                </>
              ) : (
                <>
                  <button
                    type="button"
                    className="account-login-main"
                    onClick={() => {
                      setAccountOpen(false)

                      navigate('/login')
                    }}
                  >
                    <LogIn size={17} />
                    Sign In
                  </button>

                  <p className="account-new-user">
                    New customer?{' '}
                    <button
                      type="button"
                      onClick={() => {
                        setAccountOpen(false)

                        navigate('/signup')
                      }}
                    >
                      Create account
                    </button>
                  </p>
                </>
              )}
            </div>
          )}
        </div>

        {/* ORDERS */}

        <button
          type="button"
          className="header-action"
          onClick={() => {
            if (isAuthenticated) {
              navigate('/orders')
            } else {
              navigate('/login?returnUrl=/orders')
            }
          }}
        >
          <PackageSearch size={22} />

          <div className="header-action-copy">
            <small>Your</small>

            <strong>Orders</strong>
          </div>
        </button>

        {/* CART */}

        <button
          type="button"
          className="store-cart-button"
          onClick={() => {
            if (isAuthenticated) {
              navigate('/cart')
            } else {
              navigate('/login?returnUrl=/cart')
            }
          }}
        >
          <div className="cart-icon-wrap">
            <ShoppingBag size={25} />

            {cartCount > 0 && (
              <span className="store-cart-count">{cartCount > 99 ? '99+' : cartCount}</span>
            )}
          </div>

          <strong>Cart</strong>
        </button>
      </div>

      {/* CATEGORY NAV */}

      <nav className={mobileOpen ? 'store-category-nav open' : 'store-category-nav'}>
        <div className="store-category-nav-inner">
          <button
            type="button"
            className="category-nav-featured"
            onClick={() => scrollToSection('merchandise')}
          >
            Shop
          </button>

          <button type="button" onClick={() => scrollToSection('teams')}>
            Teams
          </button>

          <button type="button" onClick={() => shopType('Jersey')}>
            Jerseys
          </button>

          <button type="button" onClick={() => shopType('Cap')}>
            Caps
          </button>

          <button type="button" onClick={() => shopType('Flag')}>
            Flags
          </button>

          <button type="button" onClick={() => shopType('AutographedPhoto')}>
            Autographed
          </button>
        </div>
      </nav>
    </header>
  )
}

export default Header
