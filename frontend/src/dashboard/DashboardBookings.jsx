import { useEffect, useState } from "react";
import request from "../services/api";
import "./DashboardBookings.css";

function DashboardBookings() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Fetch bookings for the logged-in customer
  useEffect(() => {
    async function loadBookings() {
      try {
        const data = await request("/Booking/my-bookings");

        console.log("My Bookings API response:", data);

        setBookings(Array.isArray(data) ? data : []);
      } catch (err) {
        console.error("Failed to load bookings:", err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadBookings();
  }, []);

  return (
    <div className="dashboard-bookings">
      <h2>My Bookings</h2>
      <p>View and manage your yoga class bookings.</p>

      {loading && <p>Loading your bookings...</p>}

      {error && <p className="error-message">{error}</p>}

      {!loading && !error && bookings.length === 0 && (
        <div className="empty-bookings">
          <h3>No Bookings Yet</h3>
          <p>
            You haven't booked any yoga classes yet.
            Explore our programs and classes to get started!
          </p>
        </div>
      )}

      {!loading && !error && bookings.length > 0 && (
        <div className="booking-list">
          {bookings.map((booking) => (
            <article className="booking-card" key={booking.id}>
              <h3>Booking #{booking.id}</h3>

              <p>
                <strong>Class ID:</strong> {booking.yogaClassId}
              </p>

              <p>
                <strong>Status:</strong> {booking.status}
              </p>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

export default DashboardBookings;