import { Navigate, useLocation } from 'react-router-dom'

import { useAuth } from '../context/AuthContext'

function ProtectedRoute({ children }) {
  const location = useLocation()

  const { isAuthenticated, loading } = useAuth()

  /*
   * Wait until AuthContext has
   * checked the stored JWT.
   *
   * Otherwise refreshing /cart
   * could briefly redirect to login
   * before authentication finishes.
   */
  if (loading) {
    return (
      <main className="route-loading">
        <div className="route-loading-card">
          <div className="route-spinner" />

          <span>Checking your account...</span>
        </div>
      </main>
    )
  }

  if (!isAuthenticated) {
    const returnUrl = `${location.pathname}${location.search}`

    return <Navigate to={`/login?returnUrl=${encodeURIComponent(returnUrl)}`} replace />
  }

  return children
}

export default ProtectedRoute
