const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7096/api'

export const apiRequest = async (path, options = {}, token = null) => {
  const headers = {
    ...(options.body
      ? {
          'Content-Type': 'application/json',
        }
      : {}),

    ...options.headers,
  }

  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  })

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
    const error = new Error(
      data?.message || data?.title || `Request failed with status ${response.status}`
    )

    error.status = response.status

    error.data = data

    throw error
  }

  return data
}
