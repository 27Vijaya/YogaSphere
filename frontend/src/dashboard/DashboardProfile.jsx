import { useEffect, useState } from "react";
import request from "../services/api";

function DashboardProfile() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Load the currently logged-in user's details
  useEffect(() => {
    async function loadProfile() {
      try {
        const data = await request("/Users/me");
        setUser(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadProfile();
  }, []);

  if (loading) {
    return <p>Loading your profile...</p>;
  }

  if (error) {
    return <p className="error-message">{error}</p>;
  }

  if (!user) {
    return <p>Profile information is unavailable.</p>;
  }

  return (
    <div className="dashboard-profile">
      <h2>My Profile</h2>
      <p>View your account information.</p>

      <div className="profile-details">
        <p><strong>First Name:</strong> {user.firstName}</p>
        <p><strong>Last Name:</strong> {user.lastName}</p>
        <p><strong>Email:</strong> {user.email}</p>
        <p><strong>Role:</strong> {user.role}</p>
      </div>
    </div>
  );
}

export default DashboardProfile;