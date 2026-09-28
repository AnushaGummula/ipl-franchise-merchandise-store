import { createContext, useContext, useEffect, useMemo, useState } from 'react'

import { getCurrentUser, loginUser, registerUser } from '../api/authApi'

const AuthContext = createContext(null)

const AUTH_STORAGE_KEY = 'ipl-auth'

export function AuthProvider({ children }) {
  const [auth, setAuth] = useState(() => {
    try {
      const saved = sessionStorage.getItem(AUTH_STORAGE_KEY)

      return saved ? JSON.parse(saved) : null
    } catch {
      return null
    }
  })

  const [loading, setLoading] = useState(true)

  const saveAuth = value => {
    setAuth(value)

    if (value) {
      sessionStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(value))
    } else {
      sessionStorage.removeItem(AUTH_STORAGE_KEY)
    }
  }

  useEffect(() => {
    const validateSession = async () => {
      if (!auth?.token) {
        setLoading(false)
        return
      }

      try {
        if (auth.expiresAt && new Date(auth.expiresAt) <= new Date()) {
          saveAuth(null)
          return
        }

        const user = await getCurrentUser(auth.token)

        saveAuth({
          ...auth,

          userId: user.userId,

          fullName: user.fullName,

          email: user.email,
        })
      } catch {
        saveAuth(null)
      } finally {
        setLoading(false)
      }
    }

    validateSession()

    // run once on application load
    // eslint-disable-next-line
  }, [])

  const login = async (email, password) => {
    const response = await loginUser({
      email,
      password,
    })

    saveAuth(response)

    return response
  }

  const register = async (fullName, email, password) => {
    const response = await registerUser({
      fullName,
      email,
      password,
    })

    saveAuth(response)

    return response
  }

  const logout = () => {
    saveAuth(null)
  }

  const value = useMemo(
    () => ({
      token: auth?.token ?? null,

      user: auth
        ? {
            userId: auth.userId,

            fullName: auth.fullName,

            email: auth.email,
          }
        : null,

      isAuthenticated: Boolean(auth?.token),

      loading,

      login,
      register,
      logout,
    }),
    [auth, loading]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider')
  }

  return context
}
