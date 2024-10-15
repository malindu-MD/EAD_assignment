import React, { useEffect, useState } from "react";
import { Table, Switch } from "antd";
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline } from "react-icons/ti";
import CustomModal from "../components/CustomModal";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";
import { toast } from "react-toastify";


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
   

    fetchCategories();
  }, []);

  const fetchCategories = async () => {
    try {
      const response = await axiosInstance.get(`${base_url}category`, config());
      setCategories(response.data);
    } catch (error) {
      console.error("Error fetching categories:", error);
    }
  };

  // Delete category
  const deleteProdCategory = async (id) => {
    try {
      console.log(id);
      await axiosInstance.delete(`${base_url}category/${id}`, config());
      setCategories(categories.filter((category) => category._id !== id));
      setOpen(false);
      toast.success("Category deleted successfully");
      fetchCategories();
    } catch (error) {
      console.error("Error deleting category:", error);
    }
  };

  // Toggle active status
  const handleToggleActive = async (record) => {
    console.log("Toggling active status for category:", record);
    const endpoint = record.isActive
      ? `${base_url}Category/Deactivate/${record.catid}`
      : `${base_url}Category/Activate/${record.catid}`;
      
    try {
      await axiosInstance.patch(endpoint, config());
      // Update the state to reflect the new active status
      setCategories((prevCategories) =>
        prevCategories.map((category) =>
          category._id === record.catid
            ? { ...category, isActive: !record.isActive }
            : category
        )
      );

      fetchCategories();
      toast.success(`Category ${record.isActive ? "deactivated" : "activated"} successfully`);
    } catch (error) {
      console.error("Error updating category status:", error);
    }
  };

  const data = categories.map((category, index) => ({
    key: index + 1,
    title: category.name,
    catid:category.id,
    isActive: category.isActive,
    action: (
      <>
        <Link to={''} className="text-success fs-3">
          <BiEdit />
        </Link>

        <button
          className="ms-3 text-danger fs-3 bg-transparent border-0"
          onClick={() => showModal(category.id)}
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
        <Table 
          columns={[
            {
              title: "Number",
              dataIndex: "key",
              align: "center",
            },
            {
              title: "Category Name",
              dataIndex: "title",
              align: "center",
              sorter: (a, b) => a.title.length - b.title.length,
            },
            {
              title: "Active",
              dataIndex: "isActive",
              align: "center",
              render: (isActive, record) => (
                <Switch    
                  checked={isActive}
                  onChange={() => handleToggleActive(record)}
                />
              ),
            },
            {
              title: "Action",
              dataIndex: "action",
              align: "center",
            },
          ]}
          dataSource={data} 
        />
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
