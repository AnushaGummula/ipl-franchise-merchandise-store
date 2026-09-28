import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { OrderProvider } from './context/OrderContext'

import './index.css'
import { AuthProvider } from './context/AuthContext'
import App from './App.jsx'
import { CartProvider } from './context/CartContext'

import './styles/store.css'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <OrderProvider>
          <CartProvider>
            <App />
          </CartProvider>
        </OrderProvider>
      </AuthProvider>
    </BrowserRouter>
  </StrictMode>
)
