import { Navigate } from "react-router-dom";

export const PrivateRoutes = ({ children, allowedRoles }) => {
  const user = JSON.parse(localStorage.getItem("user"));
  const hasAccess = user && allowedRoles.includes(user.role);

  return hasAccess ? children : <Navigate to="/" replace={true} />;
};
