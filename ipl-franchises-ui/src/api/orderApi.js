import { apiRequest } from './apiClient'

export const checkout = token =>
  apiRequest(
    '/orders/checkout',
    {
      method: 'POST',
    },
    token
  )

export const getOrders = token =>
  apiRequest(
    '/orders',
    {
      method: 'GET',
    },
    token
  )

export const getOrderById = (token, orderId) =>
  apiRequest(
    `/orders/${orderId}`,
    {
      method: 'GET',
    },
    token
  )
