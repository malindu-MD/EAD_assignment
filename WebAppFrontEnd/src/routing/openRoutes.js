import { Navigate } from "react-router-dom";

export const OpenRoutes = ({ children }) => {
  const user = JSON.parse(localStorage.getItem("user"));


  if(!user){

    return children;
  }else if(user.role==="Vendor"){

    return <Navigate to="/vendor" replace={true} />;
  }else if(user.role==="CSR"){
    
    return <Navigate to="/csr" replace={true} />;

  }else if(user.role==="Administrator"){
    
    return <Navigate to="/administrator" replace={true} />;

  }


  


};
