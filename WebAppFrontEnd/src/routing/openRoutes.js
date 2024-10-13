import { Navigate } from "react-router-dom";

export const OpenRoutes = ({ children }) => {
  const user = JSON.parse(localStorage.getItem("user"));
  
  return !user ? children : <Navigate to="/vendor" replace={true} />;
};
