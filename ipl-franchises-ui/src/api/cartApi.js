import { apiRequest } from './apiClient'

export const getCart = token =>
  apiRequest(
    '/cart',
    {
      method: 'GET',
    },
    token
  )

export const addCartItem = (token, productId, quantity = 1, selectedSize = null) =>
  apiRequest(
    '/cart/items',
    {
      method: 'POST',

      body: JSON.stringify({
        productId,
        quantity,
        selectedSize,
      }),
    },
    token
  )

export const updateCartItem = (token, cartItemId, quantity) =>
  apiRequest(
    `/cart/items/${cartItemId}?quantity=${quantity}`,
    {
      method: 'PUT',
    },
    token
  )

export const removeCartItem = (token, cartItemId) =>
  apiRequest(
    `/cart/items/${cartItemId}`,
    {
      method: 'DELETE',
    },
    token
  )
