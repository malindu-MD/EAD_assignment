import React, { useEffect, useState } from "react";
import { Table, Spin } from "antd";
import { useParams,useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";
import CustomModal from "../components/CustomModal";
import { FiArrowLeft } from "react-icons/fi";

const MainOrderItem = () => {
  const navigate = useNavigate();

  const columns = [
    {
      title: "Number",
      dataIndex: "key",
      align: "center",
    },
    {
      title: "Store Name",
      dataIndex: "store",
      align: "center",
      sorter: (a, b) => a.store.length - b.store.length,
    },
    {
      title: "Product Name",
      dataIndex: "pname",
      align: "center",
      sorter: (a, b) => a.pname.length - b.pname.length,
    },
    {
      title: "Product Price(Rs)",
      dataIndex: "price",
      align: "center",
    },
    {
      title: "Order Item Quantity",
      dataIndex: "quantity",
      align: "center",
    },
    {
      title: "Total(Rs)",
      dataIndex: "total",
      align: "center",
    },
    {
      title: "Order Item Status",
      dataIndex: "ostatus",
      align: "center",
      render: (status) => (
        <span style={getStatusStyle(status)}>
          {status}
        </span>
      ),
    },
  ];

  const [orders, setOrders] = useState([]);
  const { orderId,ordercode } = useParams();
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
    fetchOrdersAndCustomers();
  }, [orderId]);

  // Fetch all products
  const fetchOrdersAndCustomers = async () => {
    try {
      // Set loading to true when fetching starts
      setLoading(true);
      const ordersRes = await axiosInstance.get(`${base_url}Order/${orderId}/items`, config());
      // Set orders data
      setOrders(ordersRes.data);
    } catch (error) {
      toast.error("Error fetching data");
    } finally {
      // Set loading to false when fetching ends
      setLoading(false);
    }
  };

  const getStatusStyle = (status) => {
    let bgColor, textColor;
    switch (status) {
      case "Pending":
        bgColor = "#FFE4B5"; // Light orange background
        textColor = "#FFA500"; // Orange text
        break;
      case "Shipped":
        bgColor = "#ADD8E6"; // Light blue background
        textColor = "#1E90FF"; // Blue text
        break;
      case "Delivered":
        bgColor = "#90EE90"; // Light green background
        textColor = "#32CD32"; // Green text
        break;
      case "Cancelled":
        bgColor = "#FFC0CB"; // Light pink background
        textColor = "#FF0000"; // Red text
        break;
      default:
        bgColor = "white";
        textColor = "black";
    }

    return {
      backgroundColor: bgColor,
      color: textColor,
      padding: "5px 10px",
      borderRadius: "8px", // Rounded corners
      display: "inline-block",
      fontWeight: "bold",
    };
  };

  const data = orders.map((order, index) => ({
    key: index + 1,
    store: order.vendorName,
    pname: order.productName,
    price: `${order.productPrice}.00`,
    quantity: order.quantity,
    total: `${order.productPrice * order.quantity}.00`,
    ostatus: order.fulfillmentStatus,
  }));

  const handleBack = () => {
    navigate(-1); // Go back to the previous page
  };


  return (
    <div>
       <div className="d-flex align-items-center mb-4">
        <div
          onClick={handleBack}
          style={{
            cursor: "pointer",
            marginRight: "15px",
            fontSize: "30px",
            backgroundColor: "#007bff",
            color: "#fff",
            padding: "10px",
            borderRadius: "50%",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <FiArrowLeft />
        </div>
        <h5 className="title border border-primary p-3 bg-light text-dark">
          <span style={{ fontSize: "18px" }}>Order Items for Order ID: </span>
          <span className="text-primary" style={{ fontSize: "18px" }}>{ordercode}</span>
        </h5>
      </div>
      {loading ? ( // Show loading spinner if loading is true
        <div style={{ textAlign: 'center', marginTop: '50px' }}>
          <Spin size="large" />
        </div>
      ) : (
        <Table columns={columns} dataSource={data} />
      )}
      <CustomModal
        hideModal={hideModal}
        open={open}
        performAction={() => {
          // Your delete logic here
        }}
        title="Are you sure you want to remove the vendor?"
      />
    </div>
  );
};

export default MainOrderItem;
