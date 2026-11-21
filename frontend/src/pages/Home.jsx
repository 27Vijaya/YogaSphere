
import { useEffect, useState } from "react";
import request from "../services/api";

function Home() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Fetch the currently logged-in user's details
  useEffect(() => {
    async function loadUser() {
      try {
        const data = await request("/Users/me");
        setUser(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadUser();
  }, []);

  return (
    <div className="page-container">
      <h1>Welcome to YogaSphere</h1>

      <p>Your journey to a healthier lifestyle starts here.</p>

      {loading && <p>Loading your profile...</p>}

      {error && <p className="error-message">{error}</p>}

      {user && (
        <div>
          <h2>Welcome, {user.firstName}!</h2>
          <p>Email: {user.email}</p>
          <p>Role: {user.role}</p>
        </div>
      )}
    </div>
  );
}

export default Home;
