import { ArrowRight, Eye, EyeOff, LockKeyhole, Mail, User } from 'lucide-react'

import { useState } from 'react'

import { Link, useNavigate } from 'react-router-dom'

import Header from '../components/Header'

import { useAuth } from '../context/AuthContext'

function SignUpPage() {
  const navigate = useNavigate()

  const { register } = useAuth()

  const [fullName, setFullName] = useState('')

  const [email, setEmail] = useState('')

  const [password, setPassword] = useState('')

  const [confirmPassword, setConfirmPassword] = useState('')

  const [showPassword, setShowPassword] = useState(false)

  const [submitting, setSubmitting] = useState(false)

  const [error, setError] = useState('')

  const handleSubmit = async event => {
    event.preventDefault()

    setError('')

    if (password !== confirmPassword) {
      setError('Passwords do not match.')

      return
    }

    try {
      setSubmitting(true)

      await register(fullName.trim(), email.trim(), password)

      navigate('/', {
        replace: true,
      })
    } catch (err) {
      const errors = err.data?.errors

      if (Array.isArray(errors) && errors.length > 0) {
        setError(errors.join(' '))
      } else {
        setError(err.message || 'Unable to create account.')
      }
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

            <h1>Join the fan community.</h1>

            <p>Create an account to manage your merchandise, cart and order history.</p>

            <div className="auth-feature-list">
              <span>✓ 10 franchises</span>

              <span>✓ 40 merchandise products</span>

              <span>✓ Secure account checkout</span>
            </div>
          </div>

          <div className="auth-form-panel">
            <div className="auth-form-heading">
              <span>CREATE ACCOUNT</span>

              <h2>Sign up</h2>

              <p>Create your fan-store account.</p>
            </div>

            {error && <div className="auth-error">{error}</div>}

            <form className="auth-form" onSubmit={handleSubmit}>
              <label>Full Name</label>

              <div className="auth-input">
                <User size={17} />

                <input
                  type="text"
                  value={fullName}
                  placeholder="Your full name"
                  required
                  autoComplete="name"
                  onChange={event => setFullName(event.target.value)}
                />
              </div>

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
                  placeholder="Minimum 8 characters"
                  minLength={8}
                  required
                  autoComplete="new-password"
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

              <div className="password-hint">
                At least 8 characters, including uppercase, lowercase and a number.
              </div>

              <label>Confirm Password</label>

              <div className="auth-input">
                <LockKeyhole size={17} />

                <input
                  type={showPassword ? 'text' : 'password'}
                  value={confirmPassword}
                  placeholder="Confirm password"
                  required
                  autoComplete="new-password"
                  onChange={event => setConfirmPassword(event.target.value)}
                />
              </div>

              <button type="submit" className="auth-submit" disabled={submitting}>
                {submitting ? 'Creating account...' : 'Create Account'}

                {!submitting && <ArrowRight size={18} />}
              </button>
            </form>

            <div className="auth-switch">
              Already registered? <Link to="/login">Sign in</Link>
            </div>
          </div>
        </section>
      </main>
    </>
  )
}

export default SignUpPage
