
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";

import App from "./App.jsx";
import { AuthProvider } from "./context/AuthContext.jsx";

createRoot(document.getElementById("root")).render(
  <StrictMode>
    {/* Wrapping App with AuthProvider makes the authentication state accessible to Navbar, Login, ProtectedRoute, PublicRoute and other components. */}
    <AuthProvider>
      <App />
    </AuthProvider>
  </StrictMode>
);
