import { apiRequest } from './apiClient'

export const getProducts = async ({
  Search = '',
  Franchise = '',
  Type = '',
  PageNumber = 1,
  PageSize = 50,
} = {}) => {
  const params = new URLSearchParams()

  if (Search) {
    params.set('Search', Search)
  }

  if (Franchise) {
    params.set('Franchise', Franchise)
  }

  if (Type) {
    params.set('Type', Type)
  }

  params.set('PageNumber', PageNumber)

  params.set('PageSize', PageSize)

  const queryString = params.toString()

  return apiRequest(`/products${queryString ? `?${queryString}` : ''}`, {
    method: 'GET',
  })
}

export const getProductById = async productId => {
  return apiRequest(`/products/${productId}`, {
    method: 'GET',
  })
}
