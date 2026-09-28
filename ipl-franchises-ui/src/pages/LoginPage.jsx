import { ArrowRight, Eye, EyeOff, LockKeyhole, Mail } from 'lucide-react'

import { useState } from 'react'

import { Link, useLocation, useNavigate } from 'react-router-dom'

import Header from '../components/Header'

import { useAuth } from '../context/AuthContext'

function LoginPage() {
  const navigate = useNavigate()

  const location = useLocation()

  const { login } = useAuth()

  const [email, setEmail] = useState('')

  const [password, setPassword] = useState('')

  const [showPassword, setShowPassword] = useState(false)

  const [submitting, setSubmitting] = useState(false)

  const [error, setError] = useState('')

  const searchParams = new URLSearchParams(location.search)

  const returnUrl = searchParams.get('returnUrl') || '/'

  const handleSubmit = async event => {
    event.preventDefault()

    setError('')

    try {
      setSubmitting(true)

      await login(email.trim(), password)

      navigate(returnUrl, {
        replace: true,
      })
    } catch (err) {
      setError(err.message || 'Unable to sign in.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <>
      <Header />

      <main className="auth-page">
        <section className="auth-shell">
          <div className="auth-brand-panel">
            <img src="/favicon.svg" alt="IPL Merchandise" className="auth-logo" />

            <span className="auth-eyebrow">FAN STORE DEMO</span>

            <h1>Welcome back, cricket fan.</h1>

            <p>Sign in to access your cart, checkout and order history.</p>

            <div className="auth-feature-list">
              <span>✓ Persistent shopping cart</span>

              <span>✓ Secure checkout</span>

              <span>✓ Personal order history</span>
            </div>
          </div>

          <div className="auth-form-panel">
            <div className="auth-form-heading">
              <span>ACCOUNT</span>

              <h2>Sign in</h2>

              <p>Enter your registered account details.</p>
            </div>

            {error && <div className="auth-error">{error}</div>}

            <form className="auth-form" onSubmit={handleSubmit}>
              <label>Email Address</label>

              <div className="auth-input">
                <Mail size={17} />

                <input
                  type="email"
                  value={email}
                  placeholder="you@example.com"
                  required
                  autoComplete="email"
                  onChange={event => setEmail(event.target.value)}
                />
              </div>

              <label>Password</label>

              <div className="auth-input">
                <LockKeyhole size={17} />

                <input
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                  placeholder="Enter your password"
                  required
                  autoComplete="current-password"
                  onChange={event => setPassword(event.target.value)}
                />

                <button
                  type="button"
                  className="password-toggle"
                  onClick={() => setShowPassword(value => !value)}
                >
                  {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              </div>

              <button type="submit" className="auth-submit" disabled={submitting}>
                {submitting ? 'Signing in...' : 'Sign In'}

                {!submitting && <ArrowRight size={18} />}
              </button>
            </form>

            <div className="auth-switch">
              New customer? <Link to="/signup">Create an account</Link>
            </div>
          </div>
        </section>
      </main>
    </>
  )
}

export default LoginPage
