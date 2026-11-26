import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import request from "../services/api";
import { useAuth } from "../context/useAuth";

function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // Update form values
  function handleChange(event) {
    const { name, value } = event.target;

    setFormData((previousData) => ({
      ...previousData,
      [name]: value,
    }));
  }

  // Authenticate the user
  async function handleSubmit(event) {
    event.preventDefault();

    setError("");
    setLoading(true);

    try {
      const data = await request("/Auth/login", {
        method: "POST",
        body: JSON.stringify(formData),
      });

      // Update central authentication state
      login(data.accessToken, data.user);

     switch (data.user.role) {
  case "Trainer":
    navigate("/trainer", { replace: true });
    break;

  case "Admin":
    navigate("/admin", { replace: true });
    break;

  case "Customer":
    navigate("/dashboard", { replace: true });
    break;

  default:
    setError("Your account role is not recognized.");
    break;
}

    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="page-container">
      <div className="auth-container">
        <h1>Welcome Back!</h1>
        <p>Login to continue your YogaSphere journey.</p>

        {error && <p className="error-message">{error}</p>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email Address</label>
            <input
              type="email"
              id="email"
              name="email"
              placeholder="Enter your email"
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              name="password"
              placeholder="Enter your password"
              value={formData.password}
              onChange={handleChange}
              required
            />
          </div>

          <button
            type="submit"
            className="auth-button"
            disabled={loading}
          >
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>

        <p className="auth-link">
          Don't have an account? <Link to="/register">Register</Link>
        </p>
      </div>
    </div>
  );
}

export default Login;
