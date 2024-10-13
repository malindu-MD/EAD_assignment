import React, { useEffect, useState } from "react";
import { Table,Switch ,Select } from "antd";
import { Link ,useParams } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline, TiStar } from "react-icons/ti";
import CustomModal from "../components/CustomModal";
import axios from "axios";
import { toast } from "react-toastify";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";
const { Option } = Select;



const MainOrderItem = () => {
  const columns = [
    {
      title: "Number",
      dataIndex: "key",
    },
    {
      title: "Store Name",
      dataIndex: "store",
      sorter: (a, b) => a.productid.length - b.productid.length,
    },
    {
      title: "Product Name",
      dataIndex: "pname",
      sorter: (a, b) => a.email.length - b.email.length,
    },
    {
        title: "Product Price",
        dataIndex: "price",
    },
    {
      title: "Order Item Quantity",
      dataIndex: "quantity",
    },
    {
        title: "Total",
        dataIndex: "total",
    },
    {
        title: "Order Item Status",
        dataIndex: "ostatus",
    }
  ];

  const [orders, setOrders] = useState([]);
  const { orderId } = useParams();
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
    fetchOrdersAndCustomers();

  }, [orderId]);

  // Fetch all products
  const fetchOrdersAndCustomers = async () => {
    try {
      // Fetch orders   /api/Order/{orderId}/vendor/items
     
      const ordersRes = await axiosInstance.get(`${base_url}Order/${orderId}/items`, config());

     
      // Set both orders and customers data
      setOrders(ordersRes.data);
  
    } catch (error) {
      toast.error("Error fetching data");
    }
  };


  

 


  // Fetch all colors
  


  const data = [];
  for (let i = 0; i < orders.length; i++) {
    data.push({
      key: i + 1,
      store: orders[i].vendorName,
      pname: orders[i].productName,
      price:orders[i].productPrice + ".00",
      quantity: orders[i].quantity,
      total: (orders[i].productPrice * orders[i].quantity) +".00", 
      ostatus: orders[i].fulfillmentStatus,
      
    });
  }



 

  


  
  return (
    <div>
      <h3 className="mb-4 title">##OrderID-OD56547656656 Order Items</h3>
      <div>
        <Table  columns={columns} dataSource={data} />
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

export default MainOrderItem;
