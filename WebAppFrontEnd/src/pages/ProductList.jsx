import React, { useEffect, useState } from "react";
import { Table, Switch, Spin } from "antd"; // Import Spin
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline } from "react-icons/ti";
import CustomModal from "../components/CustomModal";
import { toast } from "react-toastify";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";

const ProductList = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [open, setOpen] = useState(false);
  const [productId, setProductId] = useState("");
  const [loading, setLoading] = useState(true); // Loading state

  const showModal = (e) => {
    setOpen(true);
    setProductId(e);
  };

  const hideModal = () => {
    setOpen(false);
  };

  useEffect(() => {
    fetchProducts();
    fetchCategories(); // Fetch categories
  }, []);

  // Fetch all products
  const fetchProducts = async () => {
    setLoading(true); // Start loading
    try {
      const res = await axiosInstance.get(`${base_url}Product/Vendor`, config());
      setProducts(res.data);
    } catch (error) {
    } finally {
      setLoading(false); // End loading
    }
  };

  // Fetch all categories
  const fetchCategories = async () => {
    try {
      const res = await axiosInstance.get(`${base_url}Category`, config());
      setCategories(res.data);
    } catch (error) {
      toast.error("Error fetching categories");
    }
  };

  const data = products.map((product, index) => {
    const category = categories.find(cat => cat.id === product.categoryId); // Get the category by ID
    return {
      key: index + 1,
      code: product.productId,
      name: product.name,
      category: category ? category.name : "Unknown", // Use category name
      stock: product.stock,
      price: `${product.price}.00`,
      isActive: product.isActive,
      id: product.id,
      action: (
        <>  
          <Link to={`/vendor/product/${product.id}`} className="text-success fs-3">
            <BiEdit />
          </Link>
          <button
            className="ms-3 text-danger fs-3 bg-transparent border-0"
            onClick={() => showModal(product.id)}
          >
            <TiDeleteOutline />
          </button>
        </>
      ),
    };
  });

  const toggleProductStatus = async (id, isActive) => {
    try {
      const endpoint = isActive
        ? `product/${id}/activate`
        : `product/${id}/deactivate`;
      
      await axiosInstance.patch(`${base_url + endpoint}`, config());

      toast.success(`Product ${isActive ? "activated" : "deactivated"} successfully`);
      fetchProducts(); // Refetch products after status change
    } catch (error) {
      toast.error("Error updating product status");
    }
  };

  const deleteAProduct = async (id) => {
    try {
      await axiosInstance.delete(`${base_url}product/${id}`, config());
      toast.success("Product deleted successfully");
      setOpen(false);
      fetchProducts(); // Refetch products after deletion
    } catch (error) {
      toast.error("Error deleting product");
    }
  };

  return (
    <div>
      <h3 className="mb-4 title">Products</h3>
      {loading ? ( // Show loading spinner while fetching data
        <div className="d-flex justify-content-center align-items-center" style={{ height: "100vh" }}>
          <Spin size="large" /> {/* Ant Design loading spinner */}
        </div>
      ) : (
        <Table columns={[
          {
            title: "Number",
            dataIndex: "key",
            align: "center",
          },
          {
            title: "Product Code",
            dataIndex: "code",
            align: "center",
            sorter: (a, b) => a.code.length - b.code.length,
          },
          {
            title: "Product Name",
            dataIndex: "name",
            align: "center",
            sorter: (a, b) => a.name.length - b.name.length,
          },
          {
            title: "Category",
            dataIndex: "category",
            align: "center",
            sorter: (a, b) => a.category.length - b.category.length,
          },
          {
            title: "Stock",
            dataIndex: "stock",
            align: "center",
          },
          {
            title: "Price(Rs)",
            dataIndex: "price",
            sorter: (a, b) => a.price - b.price,
            align: "center",
          },
          {
            title: "Active",
            dataIndex: "isActive",
            align: "center",
            render: (isActive, record) => (
              <Switch
                checked={isActive}
                onChange={() => toggleProductStatus(record.id, !isActive)}
              />
            ),
          },
          {
            title: "Action",
            dataIndex: "action",
            align: "center",
          },
        ]} dataSource={data} />
      )}
      <CustomModal
        hideModal={hideModal}
        open={open}
        performAction={() => {
          deleteAProduct(productId);
        }}
        title="Are you sure you want to delete the Product?"
      />
    </div>
  );
};

export default ProductList;
