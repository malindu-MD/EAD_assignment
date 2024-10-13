import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useFormik } from "formik";
import * as Yup from "yup";
import CustomInput from "../components/CustomInput";
import { HiOutlineArrowLongLeft } from "react-icons/hi2";
import axios from "axios";
import { toast } from "react-toastify";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";

// Hardcoded token (for temporary use)

// Yup validation schema
const schema = Yup.object().shape({
  title: Yup.string().required("Category Name is Required"),
});

const AddCategory = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [categoryName, setCategoryName] = useState("");

  const getCategoryId = location.pathname.split("/")[3];

  useEffect(() => {
    // If it's edit mode, load the category data
    if (getCategoryId) {
      axiosInstance
        .get(`${base_url}category/${getCategoryId}`,config())
        .then((response) => {
          setCategoryName(response.data.name);
        })
        .catch((error) => {
          toast.error("Error loading category data");
        });
    }
  }, [getCategoryId]);

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      title: categoryName || "",
    },
    validationSchema: schema,
    onSubmit: (values) => {
      if (getCategoryId) {
        // Update category
        axiosInstance
          .put(
            `${base_url}category/${getCategoryId}`,
            { name: values.title },config())
          .then(() => {
            toast.success("Category Updated Successfully");
            navigate("/admin/category-list");
          })
          .catch((error) => {
            toast.error("Error updating category");
          });
      } else {
       
        // Add new category
        axiosInstance
          .post(
            `${base_url}category`,
            { name: values.title },config())
          .then(() => {
            toast.success("Category Added Successfully");
            formik.resetForm();
            navigate("/admin/category-list");
          })
          .catch((error) => {
            console.log(values.title)
            console.log(error);
            toast.error("Error adding category");
          });
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
          {getCategoryId !== undefined ? "Edit" : "Add"} Category
        </h3>
        <div>
          {getCategoryId !== undefined && (
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
        <form
          action=""
          onSubmit={formik.handleSubmit}
          className="d-flex gap-3 flex-column"
        >
          <CustomInput
            type="text"
            label="Enter Category Name"
            name="title"
            onChng={formik.handleChange("title")}
            onBLr={formik.handleBlur("title")}
            val={formik.values.title}
          />
          <div className="error">
            {formik.touched.title && formik.errors.title}
          </div>

          <button
            className="btn btn-success border-0 rounded-3 my-5"
            type="submit"
          >
            {getCategoryId !== undefined ? "Update" : "Add"} Category
          </button>
        </form>
      </div>
    </div>
  );
};

export default AddCategory;
