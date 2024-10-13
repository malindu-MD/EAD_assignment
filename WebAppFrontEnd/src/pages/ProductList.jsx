import React, { useEffect, useState } from "react";
import { Table, Switch, Spin } from "antd"; // Import Spin
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline } from "react-icons/ti";
import CustomModal from "../components/CustomModal";
import axios from "axios";
import { toast } from "react-toastify";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";


const ProductList = () => {
  const columns = [
    {
      title: "Number",
      dataIndex: "key",
    },
    {
      title: "Product Code",
      dataIndex: "code",
      sorter: (a, b) => a.code.length - b.code.length,
    },
    {
      title: "Product Name",
      dataIndex: "name",
      sorter: (a, b) => a.name.length - b.name.length,
    },
    {
      title: "Category",
      dataIndex: "category",
      sorter: (a, b) => a.category.length - b.category.length,
    },
    {
      title: "Stock",
      dataIndex: "stock",
    },
    {
      title: "Price",
      dataIndex: "price",
      sorter: (a, b) => a.price - b.price,
    },
    {
      title: "Active",
      dataIndex: "isActive",
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
    },
  ];

  const [products, setProducts] = useState([]);
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
  }, []);

  // Fetch all products
  const fetchProducts = async () => {
    setLoading(true); // Start loading
    try {
      const res = await axiosInstance.get(`${base_url}Product/Vendor`, config()); // Replace with actual product API
      setProducts(res.data);
    } catch (error) {
      toast.error("Error fetching products");
    } finally {
      setLoading(false); // End loading
    }
  };

  const data = [];
  for (let i = 0; i < products.length; i++) {
    data.push({
      key: i + 1,
      code: products[i].productId,
      name: products[i].name,
      category: products[i].name, // Updated to use actual category
      stock: products[i].stock,
      price: products[i].price + ".00",
      isActive: products[i].isActive,
      id: products[i].id,
      action: (
        <>
          <Link
            to={`/admin/product/${products[i]._id}`}
            className="text-success fs-3"
          >
            <BiEdit />
          </Link>
          <button
            className="ms-3 text-danger fs-3 bg-transparent border-0"
            onClick={() => showModal(products[i].id)}
          >
            <TiDeleteOutline />
          </button>
        </>
      ),
    });
  }

  const toggleProductStatus = async (id, isActive) => {
    try {
      const endpoint = isActive
        ? `product/${id}/activate`
        : `product/${id}/deactivate`;
      
      await axiosInstance.patch(`${base_url + endpoint}`,config());

      toast.success(`Product ${isActive ? "activated" : "deactivated"} successfully`);
      fetchProducts(); // Refetch products after status change
    } catch (error) {
      toast.error("Error updating product status");
    }
  };

  const deleteAProduct = async (id) => {
    try {
      await axiosInstance.delete(`${base_url}product/${id}`,config()); // Replace with actual delete product API
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
          <Spin size="large"  /> {/* Ant Design loading spinner */}
        </div>
      ) : (
        <Table columns={columns} dataSource={data} />
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
