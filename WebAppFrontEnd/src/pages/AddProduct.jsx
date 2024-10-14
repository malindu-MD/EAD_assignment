import React, { useEffect, useState } from "react";
import { useLocation, useNavigate ,useParams} from "react-router-dom";
import CustomInput from "../components/CustomInput";
import ReactQuill from "react-quill";
import "react-quill/dist/quill.snow.css";
import { useFormik } from "formik";
import * as Yup from "yup";
import { Select } from "antd";
import Dropzone from "react-dropzone";
import axios from "axios";
import { toast } from "react-toastify";
import { HiOutlineArrowLongLeft } from "react-icons/hi2";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";

// Validation schema using Yup
let schema = Yup.object().shape({
  Name: Yup.string().required("Name is Required"),
  Description: Yup.string().required("Description is Required"),
  price: Yup.number().required("Price is Required"),
  stock: Yup.number().required("Stock Threshold is Required"),
  
  category: Yup.string().required("Category is Required"),
 
  quantity: Yup.number().required("Stock is Required"),
});

const cloudinaryUrl = "https://api.cloudinary.com/v1_1/dhf95uhla/image/upload";
const uploadPreset = "my_preset"; // Replace with your Cloudinary upload preset

const AddProduct = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { getProductId } = useParams();

  const [categories, setCategories] = useState([]);
  const [color, setColor] = useState([]);
  const [image, setImage] = useState(null); // Store only one image
  const [productDetails, setProductDetails] = useState(null);
  const [isImageUploaded, setIsImageUploaded] = useState(false);

  useEffect(() => {
    // Fetch brands, categories, and colors when the component mounts
    const fetchData = async () => {
      try {

       

        const categoriesRes = await axiosInstance.get(`${base_url}category/activecategory`,config());
        
      
        setCategories(categoriesRes.data);
     
        
        if (getProductId) {
          console.log(getProductId);
          const productRes = await axiosInstance.get(`${base_url}product/${getProductId}`,config());
          setProductDetails(productRes.data);
          setImage(productRes.data.imageUrl);
          setIsImageUploaded(true);
         
        }
      } catch (error) {
        toast.error("Error fetching data");
      }
    };
    
    fetchData();
  }, [getProductId]);

  useEffect(() => {
    if (productDetails) {
      formik.setValues({
        Name: productDetails.name || "",
        Description: productDetails.description || "",
        price: productDetails.price || "",
        brand: productDetails.brand || "",
        stock: productDetails.stockThreshold || "",
        category: productDetails.categoryId || "",
        quantity: productDetails.stock || "",
        image:productDetails.imageUrl || "",
      });
    }
  }, [productDetails]);

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      Name: productDetails?.name || "",
      Description: productDetails?.description || "",
      price: productDetails?.price || "",
      stock:  productDetails?.stockThreshold || "",
      category: productDetails?.categoryId || "",
      quantity:productDetails?.stock || "",
      image:productDetails?.imageUrl || "",
    },
    validationSchema: schema,
    onSubmit: async (values) => {
      try {
        if (getProductId) {
          await axiosInstance.put(`${base_url}product/${getProductId}`,{

            
              name: values.Name,
              description: values.Description,
              categoryId: values.category,
              price:values.price ,
              stock: values.quantity,
              stockThreshold: values.stock,
              imageUrl: image,
              isActive: productDetails.isActive
            

          },config());
          toast.success("Product Updated Successfully");
        } else {
          await axiosInstance.post(
            `${base_url}product`,
            {
              name: values.Name,
              description: values.Description,
              categoryId: values.category,
              price:values.price ,
              stock: values.quantity,
              stockThreshold: values.stock,
              imageUrl: image,
              isActive: true
            },
            config()
          );
          toast.success("Product Added Successfully");
        }
        navigate("/vendor/product-list");
      } catch (error) {
        toast.error("Something went wrong!");
      }
    },
  });



  
  const handleImageUpload = async (acceptedFiles) => {
    const file = acceptedFiles[0];
    const formData = new FormData();

    formData.append("file", file);
    formData.append("upload_preset", uploadPreset);

    try {
      const res = await axios.post(cloudinaryUrl, formData);
      const uploadedImage = { url: res.data.secure_url, public_id: res.data.public_id };
      setImage(uploadedImage.url); // Replace with the new image
      setIsImageUploaded(true); // Mark image as uploaded
      toast.success("Image uploaded successfully");
     
    } catch (error) {
      toast.error("Error uploading image");
      console.log(error);

      
    }
  };

  const handleImageDelete = () => {
    setImage(null); // Remove the image
    setIsImageUploaded(false); // Reset image upload status
  };

  const goBack = () => {
    navigate(-1);
  };



  return (
    <div>
      <div className="d-flex justify-content-between align-items-center">
        <h3 className="mb-4 title">{getProductId ? "Edit" : "Add"} Product</h3>
        <div>
          {getProductId && (
            <button className="bg-transparent border-0 mb-0 fs-6 d-flex gap-3 align-items-center" onClick={goBack}>
              <HiOutlineArrowLongLeft className="text-dark fs-2" />
              Go Back
            </button>
          )}
        </div>
      </div>

      <div>
        <form onSubmit={formik.handleSubmit} className="d-flex gap-3 flex-column">
          <CustomInput
            type="text"
            label="Enter Product Name"
            name="Name"
            onChng={formik.handleChange("Name")}
            onBLr={formik.handleBlur("Name")}
            val={formik.values.Name}
          />
          <div className="error">{formik.touched.Name && formik.errors.Name}</div>

          <div className="">
          <CustomInput
            type="text"
            label="Enter Product Description"
            name="Description"
            onChng={formik.handleChange("Description")}
            onBLr={formik.handleBlur("Description")}
            val={formik.values.Description}
          />
            <div className="error">{formik.touched.Description && formik.errors.Description}</div>
          </div>

          <CustomInput type="number" label="Enter Product Price" name="price" onChng={formik.handleChange("price")} onBLr={formik.handleBlur("price")} val={formik.values.price} />
          <div className="error">{formik.touched.price && formik.errors.price}</div>

          

          <select name="category" onChange={formik.handleChange("category")} onBlur={formik.handleBlur("category")} value={formik.values.category} className="form-control py-3 mb-3">
            <option value="">Select Category</option>
            {categories.map((category, index) => (
              <option key={index} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
          <div className="error">{formik.touched.category && formik.errors.category}</div>

        

         

          <CustomInput type="number" label="Enter Product Stock" name="quantity" onChng={formik.handleChange("quantity")} onBLr={formik.handleBlur("quantity")} val={formik.values.quantity} />
          <div className="error">{formik.touched.quantity && formik.errors.quantity}</div>
          
          

<CustomInput type="number" label="Enter Product Stock Threshold" name="stock" onChng={formik.handleChange("stock")} onBLr={formik.handleBlur("stock")} val={formik.values.stock} />
<div className="error">{formik.touched.stock && formik.errors.stock}</div>




     

         

          
          <div className="bg-white border-1 p-5 text-center">
            <Dropzone onDrop={(acceptedFiles) => handleImageUpload(acceptedFiles)}>
              {({ getRootProps, getInputProps }) => (
                <section>
                  <div {...getRootProps()}>
                    <input {...getInputProps()} />
                    <p>Drag 'n' drop an image here, or click to select a file</p>
                  </div>
                </section>
              )}
            </Dropzone>
          </div>

          {image && (
           <div className="position-relative mt-3" style={{ textAlign: 'center' }}>
           <button
             type="button"
             onClick={() => handleImageDelete()}
             className="btn-close position-absolute"
             style={{ top: "10px", right: "10px" }}
           ></button>
           <img 
             src={image} 
             alt="productImg" 
             width={350} 
             height={300} 
             style={{ display: 'inline-block' }} // Optional: keeps the image inline
           />
         </div>
         
          )}


          <button  disabled={!isImageUploaded}  className="btn btn-success border-0 rounded-3 my-5" type="submit">
            {getProductId ? "Update" : "Add"} Product
          </button>
        </form>
      </div>
    </div>
  );
};

export default AddProduct;
