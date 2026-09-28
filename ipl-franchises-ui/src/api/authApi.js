const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7096/api'

const handleResponse = async response => {
  const text = await response.text()

  let data = null

  if (text) {
    try {
      data = JSON.parse(text)
    } catch {
      data = text
    }
  }

  if (!response.ok) {
    const error = new Error(data?.message || data?.title || 'Something went wrong.')

    error.status = response.status
    error.data = data

    throw error
  }

  return data
}

export const registerUser = async ({ fullName, email, password }) => {
  const response = await fetch(`${API_BASE_URL}/auth/register`, {
    method: 'POST',

    headers: {
      'Content-Type': 'application/json',
    },

    body: JSON.stringify({
      fullName,
      email,
      password,
    }),
  })

  return handleResponse(response)
}

export const loginUser = async ({ email, password }) => {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',

    headers: {
      'Content-Type': 'application/json',
    },

    body: JSON.stringify({
      email,
      password,
    }),
  })

  return handleResponse(response)
}

export const getCurrentUser = async token => {
  const response = await fetch(`${API_BASE_URL}/auth/me`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  })

  return handleResponse(response)
}
