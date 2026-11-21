import { BrowserRouter, Routes, Route } from "react-router-dom";

import Home from "./pages/Home";
import Login from "./pages/Login";
import Register from "./pages/Register";

import Navbar from "./components/Navbar";
import ProtectedRoute from "./components/ProtectedRoute";
import PublicRoute from "./components/PublicRoute";

import DashboardLayout from "./dashboard/DashboardLayout";
import DashboardOverview from "./dashboard/DashboardOverview";
import DashboardProfile from "./dashboard/DashboardProfile";
import DashboardPrograms from "./dashboard/DashboardPrograms";
import DashboardClasses from "./dashboard/DashboardClasses";
import DashboardBookings from "./dashboard/DashboardBookings";

function App() {
  return (
    <BrowserRouter>
      <Navbar />

      <Routes>
        {/* Existing protected Home page */}
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <Home />
            </ProtectedRoute>
          }
        />

        {/* Dashboard and its nested pages */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <DashboardLayout />
            </ProtectedRoute>
          }
        >
          {/* Default dashboard page */}
          <Route index element={<DashboardOverview />} />

          <Route path="profile" element={<DashboardProfile />} />
          <Route path="program" element={<DashboardPrograms/>} />
          <Route path="classes" element={<DashboardClasses/>} />
          <Route path="bookings" element={<DashboardBookings/>}/>
        </Route>

        {/* Public pages - accessible only when logged out */}
        <Route
          path="/login"
          element={
            <PublicRoute>
              <Login />
            </PublicRoute>
          }
        />

        <Route
          path="/register"
          element={
            <PublicRoute>
              <Register />
            </PublicRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;