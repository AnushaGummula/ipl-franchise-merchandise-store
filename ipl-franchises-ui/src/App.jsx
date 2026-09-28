import { Navigate, Route, Routes } from 'react-router-dom'

import ProductsPage from './pages/ProductsPage'

import ProductDetailsPage from './pages/ProductDetailsPage'

import CartPage from './pages/CartPage'

import OrdersPage from './pages/OrdersPage'

import LoginPage from './pages/LoginPage'

import SignUpPage from './pages/SignUpPage'

import ProtectedRoute from './components/ProtectedRoute'

import './styles/store.css'

function App() {
  return (
    <Routes>
      {/* PUBLIC HOME */}

      <Route path="/" element={<ProductsPage />} />

      {/* PUBLIC PRODUCT DETAILS */}

      <Route path="/products/:productId" element={<ProductDetailsPage />} />

      {/* LOGIN */}

      <Route path="/login" element={<LoginPage />} />

      {/* SIGNUP */}

      <Route path="/signup" element={<SignUpPage />} />

      {/* PROTECTED CART */}

      <Route
        path="/cart"
        element={
          <ProtectedRoute>
            <CartPage />
          </ProtectedRoute>
        }
      />

      {/* PROTECTED ORDERS */}

      <Route
        path="/orders"
        element={
          <ProtectedRoute>
            <OrdersPage />
          </ProtectedRoute>
        }
      />

      {/* UNKNOWN URL */}

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App
