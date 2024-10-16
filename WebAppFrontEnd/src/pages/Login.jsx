import React, { useEffect, useState } from "react";
import CustomInput from "../components/CustomInput";
import { useNavigate } from "react-router-dom";
import { useFormik } from "formik";
import * as Yup from "yup";
import axios from "axios"; // Import axios
import { axiosInstance } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";

let schema = Yup.object().shape({
  email: Yup.string()
    .email("Email should be valid")
    .required("Email is Required"),
  password: Yup.string().required("Password is Required"),
});

const Login = () => {
  const navigate = useNavigate();
  const [errorMessage, setErrorMessage] = useState("");
  const [loading, setLoading] = useState(false);

  const formik = useFormik({
    initialValues: {
      email: "",
      password: "",
    },
    validationSchema: schema,
    onSubmit: async (values) => {
      setLoading(true);
      try {
        const response = await axiosInstance.post(`${base_url}Auth/login`, values);

        const data = response.data;

        if (response.status === 200) {
          // If login is successful, store the token and navigate to the admin page
          localStorage.setItem("user", JSON.stringify(data)); // Assuming your backend returns a JWT token
         
          if(data.role==='Vendor'){
            navigate("/vendor");
          }
          else if(data.role==='Administrator'){
            navigate("/administrator");
            }
          else{
            navigate("/csr");
          }

          formik.resetForm();
        } else {
          setErrorMessage(data.message || "Login failed. Please try again.");
        }
      } catch (error) {
        setErrorMessage("Something went wrong. Please try again later.");
      } finally {
        setLoading(false);
      }
    },
  });

  useEffect(() => {
    // Clear the error message when the form values change
    setErrorMessage("");
  }, [formik.values]);

  return (
    <div className="py-5" style={{ background: "#488A99", minHeight: "100vh" }}>
      <br />
      <br />
      <br />
      <br />
      <br />
      <div className="my-5 w-25 bg-white rounded-3 mx-auto p-4">
        <h3 className="text-center title">Admin/CSR/Vendor Login</h3>
        <p className="text-center">Login to your account to continue.</p>
        <div className="error text-center">
          {errorMessage}
        </div>
        <form action="" onSubmit={formik.handleSubmit}>
          <CustomInput
            type="text"
            label="Email Address"
            id="email"
            name="email"
            onChng={formik.handleChange("email")}
            onBlr={formik.handleBlur("email")}
            val={formik.values.email}
          />
          <div className="error mt-2">
            {formik.touched.email && formik.errors.email}
          </div>
          <CustomInput
            type="password"
            label="Password"
            id="pass"s
            name="password"
            onChng={formik.handleChange("password")}
            onBlr={formik.handleBlur("password")}
            val={formik.values.password}
          />
          <div className="error mt-2">
            {formik.touched.password && formik.errors.password}
          </div>

          <button
            className="border-0 px-3 py-2 text-white fw-bold w-100 text-center text-decoration-none fs-5"
            type="submit"
            style={{ background: "#488A99" }}
            disabled={loading}
          >
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>
      </div>
    </div>
  );
};

export default Login;
