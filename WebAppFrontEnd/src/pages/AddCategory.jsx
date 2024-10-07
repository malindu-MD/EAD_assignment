import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useFormik } from "formik";
import * as Yup from "yup";
import CustomInput from "../components/CustomInput";
import { HiOutlineArrowLongLeft } from "react-icons/hi2";
import axios from "axios";
import { toast } from "react-toastify";

// Hardcoded token (for temporary use)
const hardcodedToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI2NzAyYzI1ZGM4MjY1YzEzODJlNjhmZDEiLCJlbWFpbCI6ImJkc21AZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwMjcyMjI2MDg4Iiwicm9sZSI6IkFkbWluaXN0cmF0b3IiLCJuYmYiOjE3MjgyNzkwNzksImV4cCI6MTcyODg4Mzg3OSwiaWF0IjoxNzI4Mjc5MDc5fQ.Fzl0D4Rg3KgslPTWCP9DadT_cpRhoxx18Kk0fQR0I38'; // Replace this with your actual token

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
      axios
        .get(`http://localhost:5272/api/category/${getCategoryId}`, {
          headers: {
            Authorization: `Bearer ${hardcodedToken}`, // Using the hardcoded token
          },
        })
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
        axios
          .put(
            `http://localhost:5272/api/category/${getCategoryId}`,
            { name: values.title },
            {
              headers: {
                Authorization: `Bearer ${hardcodedToken}`, // Using the hardcoded token
              },
            }
          )
          .then(() => {
            toast.success("Category Updated Successfully");
            navigate("/admin/category-list");
          })
          .catch((error) => {
            toast.error("Error updating category");
          });
      } else {
       
        // Add new category
        axios
          .post(
            "http://localhost:5272/api/category",
            { name: values.title },
            {
              headers: {
                Authorization: `Bearer ${hardcodedToken}`, // Using the hardcoded token
                "Content-Type": "application/json",
              },
            }
          )
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
