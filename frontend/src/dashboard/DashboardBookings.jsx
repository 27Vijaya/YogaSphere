
import { useEffect, useState } from "react";
import request from "../services/api";
import "./DashboardBookings.css";

// Converts the numeric BookingStatus enum into readable text.
// Confirm these values against your BookingStatus enum.
const bookingStatusLabels = {
  1: "Pending",
  2: "Confirmed",
  3: "Cancelled",
  4: "Completed",
};

function DashboardBookings() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [cancellingId, setCancellingId] = useState(null);

  // Fetch bookings belonging to the logged-in customer.
  async function loadBookings() {
    try {
      const data = await request("/Booking/my-bookings");
      setError("");
      setBookings(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error("Failed to load bookings:", err);
      setError(err.message || "Unable to load bookings.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
  let isActive = true;

  request("/Booking/my-bookings")
    .then((data) => {
      if (isActive) {
        setError("");
        setBookings(Array.isArray(data) ? data : []);
      }
    })
    .catch((err) => {
      console.error("Failed to load bookings:", err);

      if (isActive) {
        setError(err.message || "Unable to load bookings.");
      }
    })
    .finally(() => {
      if (isActive) {
        setLoading(false);
      }
    });

  // Prevent state updates after the component unmounts.
  return () => {
    isActive = false;
  };
}, []);

  // Cancels a booking belonging to the logged-in customer.
  async function handleCancel(bookingId) {
    const confirmed = window.confirm(
      "Are you sure you want to cancel this booking?"
    );

    if (!confirmed) return;

    try {
      setCancellingId(bookingId);

      await request(`/Booking/${bookingId}/cancel`, {
        method: "PUT",
      });

      // Refresh bookings to display the updated status.
      await loadBookings();
    } catch (err) {
      console.error("Failed to cancel booking:", err);
      setError(err.message || "Unable to cancel booking.");
    } finally {
      setCancellingId(null);
    }
  }

  // Formats a date/time value returned by the API.
  function formatDate(value) {
    if (!value) return "Not available";

    return new Date(value).toLocaleDateString("en-IN", {
      day: "numeric",
      month: "short",
      year: "numeric",
    });
  }

  function formatTime(value) {
    if (!value) return "Not available";

    return new Date(value).toLocaleTimeString("en-IN", {
      hour: "2-digit",
      minute: "2-digit",
      hour12: true,
    });
  }

  function getStatusLabel(status) {
    return bookingStatusLabels[status] ?? "Unknown";
  }

  // Assign a CSS class according to the booking status.
  function getStatusClass(status) {
    const classes = {
      1: "status-pending",
      2: "status-confirmed",
      3: "status-cancelled",
      4: "status-completed",
    };

    return classes[status] || "status-unknown";
  }

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

      {!loading && bookings.length > 0 && (
        <div className="booking-list">
          {bookings.map((booking) => (
            <article className="booking-card" key={booking.id}>
              <h3>Booking #{booking.id}</h3>

              <p>
                <strong>Yoga Class:</strong>{" "}
                {booking.yogaClassName || "Class details unavailable"}
              </p>

              <p>
                <strong>Class Date:</strong>{" "}
                {formatDate(booking.classDate)}
              </p>

              <p>
                <strong>Time:</strong>{" "}
                {formatTime(booking.startTime)} -{" "}
                {formatTime(booking.endTime)}
              </p>

              <p>
                <strong>Status:</strong>
                <span className={`booking-status ${getStatusClass(booking.status)}`}>
                  {getStatusLabel(booking.status)}
                </span>
              </p>

              {(booking.status === 1 || booking.status === 2) && (
                <button
                  type="button"
                  onClick={() => handleCancel(booking.id)}
                  disabled={cancellingId === booking.id}
                >
                  {cancellingId === booking.id
                    ? "Cancelling..."
                    : "Cancel Booking"}
                </button>
              )}
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

export default DashboardBookings;
