import React, { useEffect, useState } from "react";
import { Table,Switch } from "antd";
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline, TiStar } from "react-icons/ti";
import CustomModal from "../components/CustomModal";
import axios from "axios";
import { toast } from "react-toastify";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";




const VendorList = () => {
  const columns = [
    {
      title: "Number",
      align: "center",
      dataIndex: "key",
    },
    {
      title: "Vendor Name",
      dataIndex: "vname",
      align: "center",
      sorter: (a, b) => a.vname.length - b.vname.length,
    },
    {
      title: "Email",
      dataIndex: "email",
      align: "center",
      sorter: (a, b) => a.email.length - b.email.length,
    },
    {
      title: "Business Name",
      dataIndex: "business",
      align: "center",
      sorter: (a, b) => a.business.length - b.business.length,
    },  

    {
      title: "Average Rating[5]",
      dataIndex: "arating",
      align: "center",
    },

    {
      title: "Customer Feedback" ,
      dataIndex: "comment",
      align: "center",
    },
    {
        title: "Registered Date",
        dataIndex: "date",
        align: "center",
        sorter: (a, b) => new Date(a.date) - new Date(b.date), // Sorting function
      render: (date) => new Date(date).toLocaleDateString(), // Format date for display
    },
    {
      title: "Action",
      dataIndex: "action",
      align: "center",
    },
  ];

  const [vendors, setVendors] = useState([]);

  const [open, setOpen] = useState(false);
  const [productId, setProductId] = useState("");

  const showModal = (e) => {
    setOpen(true);
    setProductId(e);
  };

  const hideModal = () => {
    setOpen(false);
  };

  useEffect(() => {
    fetchVendor();

  }, []);

  // Fetch all products
  const fetchVendor = async () => {
    try {
      const res = await axiosInstance.get(`${base_url}Vendor`,config()); // Replace with actual product API
      setVendors(res.data);
    } catch (error) {
      toast.error("Error fetching products");
    }
  };


  

 


  // Fetch all colors
  


  const data = [];
  for (let i = 0; i < vendors.length; i++) {
    data.push({
      key: i + 1,
      vname: vendors[i].username,
      email:vendors[i].email,
      business: vendors[i].businessName,
      arating: (<>
      {vendors[i].averageRating} 
    
      </>),
      comment: (
        <button className="btn btn-primary" >
          View 
        </button>
      ), // Add 
      date: new Date(vendors[i].createdAt).toLocaleDateString(),
      action: (
        <>
          <Link
            to={`/admin/product/${vendors[i]._id}`}
            className="text-success fs-3"
          >
            <BiEdit />
          </Link>

          <button
            className="ms-3 text-danger fs-3 bg-transparent border-0"
            onClick={() => showModal(vendors[i].id)}
          >
            <TiDeleteOutline />
          </button>
        </>
      ),
    });
  }


 
 

  
  return (
    <div>
      <h3 className="mb-4 title">Vendor List</h3>
      <div>
        <Table columns={columns} dataSource={data} />
      </div>
      <CustomModal
        hideModal={hideModal}
        open={open}
        performAction={() => {
         
        }}
        title="Are you sure you want to remove the vendor?"
      />
    </div>
  );
};

export default VendorList;
