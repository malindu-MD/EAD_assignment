import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import * as Yup from "yup";
import { useFormik } from "formik";
import CustomInput from "../components/CustomInput";
import { HiOutlineArrowLongLeft } from "react-icons/hi2";
import axios from "axios";

// Validation schema
let schema = Yup.object().shape({
  title: Yup.string().required("Brand Name is Required"),
});

const AddVendor = () => {
  const [brandName, setBrandName] = useState("");
  const [isUpdating, setIsUpdating] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();
  const getBrandId = location.pathname.split("/")[3];

  const hardcodedToken = 'YOUR_TOKEN_HERE'; // Replace with your actual token

  useEffect(() => {
    if (getBrandId !== undefined) {
      fetchBrandDetails(getBrandId);
      setIsUpdating(true);
    }
  }, [getBrandId]);

  // Fetch brand details for updating
  const fetchBrandDetails = async (brandId) => {
    try {
      const res = await axios.get(`http://localhost:5272/api/brand/${brandId}`, {
        headers: {
          Authorization: `Bearer ${hardcodedToken}`,
        },
      });
      setBrandName(res.data.title);
    } catch (error) {
      toast.error("Failed to fetch brand details");
    }
  };

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      title: brandName || "",
    },
    validationSchema: schema,
    onSubmit: async (values) => {
      if (isUpdating) {
        // Update brand
        try {
          await axios.put(
            `http://localhost:5272/api/brand/${getBrandId}`,
            values,
            {
              headers: {
                Authorization: `Bearer ${hardcodedToken}`,
              },
            }
          );
          toast.success("Brand Updated Successfully!");
          navigate("/admin/brand-list");
        } catch (error) {
          toast.error("Error updating brand");
        }
      } else {
        // Create new brand
        try {
          await axios.post("http://localhost:5272/api/brand", values, {
            headers: {
              Authorization: `Bearer ${hardcodedToken}`,
            },
          });
          toast.success("Brand Added Successfully!");
          formik.resetForm();
        } catch (error) {
          toast.error("Error adding brand");
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
          {getBrandId !== undefined ? "Edit" : "Edit"}  Vendor
        </h3>
        <div>
          {getBrandId !== undefined && (
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
            {formik.touched.title && formik.errors.title}
          </div>

          <button
            className="btn btn-success border-0 rounded-3 my-5"
            type="submit"
          >
            {getBrandId !== undefined ? "Update " : "Update "}
            vendor
          </button>
        </form>
      </div>
    </div>
  );
};

export default AddVendor;
