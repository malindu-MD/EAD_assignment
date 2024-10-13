import React, { useEffect, useState } from "react";
import { Table,Switch } from "antd";
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline } from "react-icons/ti";
import CustomModal from "../components/CustomModal";
import axios from "axios";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";



const columns = [
  {
    title: "Number",
    dataIndex: "key",
  },
  {
    title: "Category Name",
    dataIndex: "title",
    sorter: (a, b) => a.title.length - b.title.length,
  },
  {
    title: "Active",
    dataIndex: "isActive",
    render: (isActive, record) => (
      <Switch
        checked={isActive}
        
      />
    ),
  },
  {
    title: "Action",
    dataIndex: "action",
  },
];

const CategoryList = () => {
  const [categories, setCategories] = useState([]);
  const [open, setOpen] = useState(false);
  const [productCategoryId, setProductCategoryId] = useState("");

  const showModal = (id) => {
    setOpen(true);
    setProductCategoryId(id);
  };

  const hideModal = () => {
    setOpen(false);
  };

  // Fetch all categories
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await axiosInstance.get(`${base_url}category`,config());
        setCategories(response.data);
        console.log(response.data);
      } catch (error) {
        console.error("Error fetching categories:", error);
      }
    };

    fetchCategories();
  }, []);

  // Delete category
  const deleteProdCategory = async (id) => {
    try {
      await axiosInstance.delete(`${base_url}category/${id}`,config());
      setCategories(categories.filter((category) => category._id !== id));
      setOpen(false);
    } catch (error) {
      console.error("Error deleting category:", error);
    }
  };

  const data = categories.map((category, index) => ({
    key: index + 1,
    title: category.name,
    action: (
      <>
        <Link to={`/admin/category/${category._id}`} className="text-success fs-3">
          <BiEdit />
        </Link>

        <button
          className="ms-3 text-danger fs-3 bg-transparent border-0"
          onClick={() => showModal(category._id)}
        >
          <TiDeleteOutline />
        </button>
      </>
    ),
  }));

  return (
    <div>
      <h3 className="mb-4 title">Product Categories</h3>
      <div>
        <Table columns={columns} dataSource={data} />
      </div>
      <CustomModal
        hideModal={hideModal}
        open={open}
        performAction={() => deleteProdCategory(productCategoryId)}
        title="Are you sure you want to delete this category?"
      />
    </div>
  );
};

export default CategoryList;
