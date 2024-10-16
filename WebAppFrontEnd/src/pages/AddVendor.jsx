import React, { useEffect, useState } from "react";
import { useLocation, useNavigate,useParams } from "react-router-dom";
import { toast } from "react-toastify";
import * as Yup from "yup";
import { useFormik } from "formik";
import CustomInput from "../components/CustomInput";
import { HiOutlineArrowLongLeft } from "react-icons/hi2";
import axios from "axios";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";

// Validation schema
let schema = Yup.object().shape({
  name: Yup.string().required("Name is Required"),
  email: Yup.string().email("Invalid email").required("Email is Required"),
  phone: Yup.string().required("Phone Number is Required"),
  password:Yup.string().required("Password is Required"),
  business: Yup.string().required("business name is Required")
 
  
});

const AddVendor = () => {
  const [brandName, setBrandName] = useState("");
  const { vendorid } = useParams();

  const [vendorDetails, setVendorDetails] = useState(null);
  
  const [isUpdating, setIsUpdating] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();


  useEffect(() => {
    if (vendorid !== undefined) {
      fetchVendorDetails(vendorid);
      setIsUpdating(true);
    }
  }, [vendorid]);

  // Fetch brand details for updating
  const fetchVendorDetails = async (vendorid) => {
    try {
      const res = await axiosInstance.get(`${base_url}Vendor/${vendorid}`,config());
      setVendorDetails(res.data);
    } catch (error) {
      toast.error("Failed to fetch vendor details");
    }
  };

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: vendorDetails?.username || "",
      email: vendorDetails?.email || "",
      phone: vendorDetails?.phoneNumber || "",
      business:  vendorDetails?.businessName || "",
      password: "",
     
    },
    validationSchema: schema,
    onSubmit: async (values) => {
      if (vendorid) {
        console.log(values);
        // Update brand
        try {
          await axiosInstance.put(
            `${base_url}Vendor/${vendorid}`,
            {

              businessName:values.business,
              username:values.name,
              email:values.email,
              phoneNumber:values.phone,
              passwordHash:values.password

            },
           config()
          );
          toast.success("Vendor Updated Successfully!");
          navigate("/administrator/vendor-list");
        } catch (error) {
          console.log(error);
          toast.error("Error updating Vendor");
        }
      } else {
        // Create new brand
        try {
          await axiosInstance.post(`${base_url}Vendor/Create`, {
            
            username:values.name,
            email:values.email,
            phoneNumber:values.phone,
            passwordHash:values.password,
            businessName:values.business,



          }, config());
          toast.success("Vendor Added Successfully!");
          formik.resetForm();
          navigate("/administrator/vendor-list");
        } catch (error) {
          toast.error("Error adding Vendor");
        }
      }
    },
  });

  const goBack = () => {
    navigate(-1);
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center">
        <h3 className="mb-4 title">
          {vendorid !== undefined ? "Edit" : "Add"}  Vendor
        </h3>
        <div>
          {vendorid !== undefined && (
            <button
              className="bg-transparent border-0 mb-0 fs-6 d-flex gap-3 align-items-center"
              onClick={goBack}
            >
              <HiOutlineArrowLongLeft className="text-dark fs-2" />
              Go Back
            </button>
          )}
        </div>
      </div>
      <div>
        <form onSubmit={formik.handleSubmit}>
          <CustomInput
            type="text"
            label="Enter Vendor User Name"
            name="name"
            onChng={formik.handleChange("name")}
            onBLr={formik.handleBlur("name")}
            val={formik.values.name}
            id="name"
          />
          <div className="error">
            {formik.touched.name && formik.errors.name}
          </div>

          <CustomInput
            type="email"
            label="Enter Vendor Email"
            name="email"
            onChng={formik.handleChange("email")}
            onBLr={formik.handleBlur("email")}
            val={formik.values.email}
            id="email"
          />
          <div className="error">
            {formik.touched.email && formik.errors.email}
          </div>


          <CustomInput
            type="text"
            label="Enter Vendor Business Name"
            name="business"
            onChng={formik.handleChange("business")}
            onBLr={formik.handleBlur("business")}
            val={formik.values.business}
            id="business"
          />
          <div className="error">
            {formik.touched.business && formik.errors.business}
          </div>

          <CustomInput
            type="tel"
            label="Enter Vendor Phone Number"
            name="phone"
            onChng={formik.handleChange("phone")}
            onBLr={formik.handleBlur("phone")}
            val={formik.values.phone}
            id="phone"
          />
          <div className="error">
            {formik.touched.phone && formik.errors.phone}
          </div>


    
            
            <CustomInput
            type="Password"
            label="Enter Vendor Account Password"
            name="password"
            onChng={formik.handleChange("password")}
            onBLr={formik.handleBlur("password")}
            val={formik.values.password}
            id="password"
          />
          <div className="error">
            {formik.touched.password && formik.errors.password}
          </div>
            
     
       

          <button
            className="btn btn-success border-0 rounded-3 my-5"
            type="submit"
          >
            {vendorid !== undefined ? "Update " : "Add "}
            vendor
          </button>
        </form>
      </div>
    </div>
  );
};

export default AddVendor;
