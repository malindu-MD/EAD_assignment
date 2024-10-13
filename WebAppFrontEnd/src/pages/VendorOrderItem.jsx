import React, { useEffect, useState } from "react";
import { Table, Select, Spin } from "antd"; // Import Spin
import { useParams, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";
import CustomModal from "../components/CustomModal";
import { FiArrowLeft } from "react-icons/fi";

const { Option } = Select;

const VendorOrderItem = () => {
  const navigate = useNavigate();
  const columns = [
    {
      title: "Number",
      dataIndex: "key",
      align: "center",
    },
    {
      title: "Product Name",
      dataIndex: "pname",
      align: "center",
      render: (text, record) => (
        <div style={{ display: "flex", alignItems: "center", justifyContent: "center" }}>
          <img
            src={record.image}
            alt={record.pname}
            style={{ width: 50, height: 50, marginRight: 8 }}
          />
          {record.pname}
        </div>
      ),
    },
    {
      title: "Product Code",
      dataIndex: "productid",
      align: "center",
      sorter: (a, b) => a.productid.length - b.productid.length,
    },
    {
      title: "Product Price(Rs)",
      dataIndex: "price",
      align: "center",
    },
    {
      title: "Order Quantity",
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
      render: (status) => {
        let bgColor = "";
        let textColor = "";
        switch (status) {
          case "Pending":
            bgColor = "#FFE4B5";
            textColor = "#FFA500";
            break;
          case "Shipped":
            bgColor = "#ADD8E6";
            textColor = "#1E90FF";
            break;
          case "Delivered":
            bgColor = "#90EE90";
            textColor = "#32CD32";
            break;
          case "Cancelled":
            bgColor = "#FFC0CB";
            textColor = "#FF0000";
            break;
          default:
            bgColor = "white";
            textColor = "black";
        }
        return (
          <span
            style={{
              backgroundColor: bgColor,
              color: textColor,
              padding: "5px 10px",
              borderRadius: "8px",
              display: "inline-block",
              fontWeight: "bold",
            }}
          >
            {status}
          </span>
        );
      },
    },
    {
      title: "Change Status",
      dataIndex: "changeStatus",
      align: "center",
      
      render: (_, record) => {
        const isDelivered = record.ostatus === "Delivered";
        const isShipped = record.ostatus === "Shipped";

        return (
          <Select
            defaultValue="Select"
            style={{ width: 120 }}
            onChange={(value) => handleStatusChange(record, value)}
            disabled={isDelivered } // Disable if already delivered or shipped
          >
            {!isShipped && <Option value="Shipped">Shipped</Option>}
            {!isDelivered && <Option value="Delivered">Delivered</Option>}
          </Select>
        );
      },
    },
  ];

  const [orders, setOrders] = useState([]);
  const { orderId, ordercode } = useParams();
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [selectedProduct, setSelectedProduct] = useState(null); // State to hold selected product for status change
  const [newStatus, setNewStatus] = useState(""); // State to hold new status

  const showModal = () => {
    setOpen(true);
  };

  const hideModal = () => {
    setOpen(false);
    setSelectedProduct(null);
    setNewStatus("");
  };

  const handleStatusChange = (product, status) => {
    setSelectedProduct(product);
    setNewStatus(status);
    showModal(); // Show the confirmation modal
  };

  const confirmStatusChange = async () => {
    try {
      let endpoint = "";
      if (newStatus === "Shipped") {
        endpoint = `${base_url}Order/${orderId}/mark-item-shipped/${selectedProduct.oid}`;
      } else if (newStatus === "Delivered") {
        endpoint = `${base_url}Order/${orderId}/product/${selectedProduct.oid}/ordritem-delivered`;
      }

      await axiosInstance.put(endpoint,config());
      toast.success(`Status changed to ${newStatus} for ${selectedProduct.pname}.`);
      hideModal(); // Hide modal after confirmation
      fetchOrdersAndCustomers(); // Optionally, refetch the orders
    } catch (error) {
      toast.error("Error changing status");
    }
  };

  useEffect(() => {
    fetchOrdersAndCustomers();
  }, [orderId]);

  const fetchOrdersAndCustomers = async () => {
    try {
      setLoading(true);
      const ordersRes = await axiosInstance.get(`${base_url}Order/${orderId}/vendor/items`, config());
      setOrders(ordersRes.data);
    } catch (error) {
      toast.error("Error fetching data");
    } finally {
      setLoading(false);
    }
  };

  const data = orders.map((order, index) => ({
    key: index + 1,
    image: order.imageUrl,
    productid: order.productCode,
    oid:order.productId,
    pname: order.productName,
    price: order.productPrice + ".00",
    quantity: order.quantity,
    total: (order.productPrice * order.quantity) + ".00",
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

      {loading ? (
        <div style={{ textAlign: "center", marginTop: "50px" }}>
          <Spin size="large" />
        </div>
      ) : (
        <div>
          <Table columns={columns} dataSource={data} />
        </div>
      )}

      <CustomModal
        hideModal={hideModal}
        open={open}
        performAction={confirmStatusChange} // Pass confirm function to performAction
        title={`Are you sure you want to change the status to ${newStatus}?`}
      />
    </div>
  );
};








export default VendorOrderItem;
