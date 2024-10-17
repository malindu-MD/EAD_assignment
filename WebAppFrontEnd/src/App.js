import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Dashboard from "./pages/Dashboard";
import Login from "./pages/Login";

import Customers from "./pages/Customers";
import CategoryList from "./pages/CategoryList";
import ProductList from "./pages/ProductList";
import AddCategory from "./pages/AddCategory";
import AddProduct from "./pages/AddProduct";
import ViewEnquiry from "./pages/ViewEnquiry";
import { PrivateRoutes } from "./routing/privateRoute";
import { OpenRoutes } from "./routing/openRoutes";
import AdminLayout from "./components/AdminLayout";
import AddVendor from "./pages/AddVendor";
import VendorList from "./pages/VendorList";
import VendorOrders from "./pages/VendorOrders";
import VendorOrderItem from "./pages/VendorOrderItem";
import MainOrderList from "./pages/MainOrderList";
import MainOrderItem from "./pages/MainOrderItem";
import VendorLayout from "./components/VendorLayout";
import CsrLayout from "./components/CsrLayout";
import NotFound from "./pages/NotFound";
import DashboardAdmin from "./pages/DashboardAdmin";
import DashboardCsr from "./pages/DashboardCsr";

function App() {
  return (
    <Router>
      <Routes>
        <Route
          path="/"
          element={
            <OpenRoutes>
              <Login />
            </OpenRoutes>
          }
        />
       

        <Route
          path="/administrator"
          element={
          <PrivateRoutes allowedRoles={["Administrator"]}>
             <AdminLayout/>
            </PrivateRoutes>
          }
        >
          <Route index element={<DashboardAdmin/>} />
          <Route path="category" element={<AddCategory />} />
          <Route path='vendor' element={<AddVendor/>}/>
          <Route path='vendor/:vendorid' element={<AddVendor/>}/>
          <Route path='vendor-list' element={<VendorList/>}/>
          <Route path='main-orders' element={<MainOrderList/>}/>
          <Route path="category-list" element={<CategoryList />} />
          <Route path='main-orders/:orderId/:ordercode' element={<MainOrderItem/>}/>



          
          
          




         
        </Route>

        <Route
          path="/vendor"
          element={
            <PrivateRoutes allowedRoles={["Vendor"]}>
             <VendorLayout/>
            </PrivateRoutes>
          }
        >
          <Route index element={<Dashboard />} />
          <Route path="category" element={<AddCategory />} />
          <Route path='vendor' element={<AddVendor/>}/>
          <Route path='vendor-list' element={<VendorList/>}/>
          <Route path='vendor-orders' element={<VendorOrders/>}/>
          <Route path='vendor-orders/:orderId/:ordercode' element={<VendorOrderItem/>}/>
          <Route path='main-orders' element={<MainOrderList/>}/>
          <Route path='main-orders/:orderId' element={<MainOrderItem/>}/>
          <Route path="product-list" element={<ProductList />} />
          <Route path="product" element={<AddProduct />} />
          <Route path="product/:getProductId" element={<AddProduct />} />




         
        </Route>

        <Route
          path="/csr"
          element={
            <PrivateRoutes allowedRoles={["CSR"]}>
             <CsrLayout/>
            </PrivateRoutes>
          }
        >
          <Route index element={<DashboardCsr/>} />
        
          
          <Route path='main-orders' element={<MainOrderList/>}/>
          <Route path='main-orders/:orderId/:ordercode' element={<MainOrderItem/>}/>
          <Route path="customers" element={<Customers />} />
       




         
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}

export default App;
