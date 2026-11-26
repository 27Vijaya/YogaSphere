import { useContext } from "react";
import { AuthContext } from "./AuthContext.js";

// Custom hook to access authentication information.
export function useAuth() {
  return useContext(AuthContext);
}