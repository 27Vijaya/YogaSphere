
// Base URL of the YogaSphere ASP.NET Core API
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

// Reusable function to send HTTP requests
async function request(endpoint, options = {}) {
  // Retrieve the JWT token saved after login
  const token = localStorage.getItem("accessToken");

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",

      // Automatically attach JWT for authenticated requests
      ...(token && {
        Authorization: `Bearer ${token}`,
      }),

      ...options.headers,
    },
  });

  // Handle responses without a body
  if (response.status === 204) {
    return null;
  }

  // Read the API response
  const data = await response.json().catch(() => null);

  // Handle unsuccessful responses
 if (!response.ok) {
  console.error("API Error:", {
    endpoint,
    status: response.status,
    response: data,
  });

  throw new Error(
    data?.message ||
    data?.title ||
    `Request failed with status ${response.status}`
  );
}

  return data;
}

export default request;
