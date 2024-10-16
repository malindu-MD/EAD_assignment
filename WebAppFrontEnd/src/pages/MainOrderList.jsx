import React, { useEffect, useState } from "react";
import { Table, Select, Modal, Input, Button, Spin } from "antd"; // Import Spin
import { Link } from "react-router-dom";
import axios from "axios";
import { toast } from "react-toastify";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";
import { FaRegComment } from "react-icons/fa"; // Import message icon

const { Option } = Select;

const MainOrderList = () => {
  const columns = [
    {
      title: "Number",
      dataIndex: "key",
      align: "center",
    },
    {
      title: "OrderID",
      dataIndex: "orderid",
      align: "center",
      sorter: (a, b) => a.orderid.length - b.orderid.length,
    },
    {
      title: "Customer Name",
      dataIndex: "cname",
      align: "center",
      sorter: (a, b) => a.cname.length - b.cname.length,
    },
    {
      title: "Delivery Address",
      dataIndex: "address",
      align: "center",
      sorter: (a, b) => a.address.length - b.address.length,
    },
    {
      title: "Phone Number",
      dataIndex: "pnumber",
      align: "center",
    },
    {
      title: "Message",
      dataIndex: "messageCount",
      align: "center",
      render: (count, record) => (
        <span>
          <FaRegComment
            size={18}
            style={{ marginRight: 0, cursor: 'pointer' }}
            onClick={() => handleMessageClick(record.message || [])}// Ensure messages is an array
          />
           
           {count===0? (<></>):(<><span className="badge bg-danger rounded-circle p-1 position-absolute">
            {count}
          </span></>)}

          
        </span>
      ),
    },
    {
      title: "Ordered Date",
      dataIndex: "date",
      align: "center",
      sorter: (a, b) => new Date(a.date) - new Date(b.date), // Sorting function
      render: (date) => new Date(date).toLocaleDateString(), // Format date for display
    },
    {
      title: "Order Status",
      dataIndex: "ostatus",
      align: "center",
      render: (status) => (
        <span style={getStatusStyle(status)}>
          {status}
        </span>
      ),
    },
    {
      title: "Order Items",
      dataIndex: "oitem",
      align: "center",
    },
    {
      title: "Change Status",
      dataIndex: "changeStatus",
      align: "center",
    },
  ];

  const [orders, setOrders] = useState([]);
  const [customers, setCustomers] = useState({});
  const [openFulfillModal, setOpenFulfillModal] = useState(false);
  const [openCancelModal, setOpenCancelModal] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [loading, setLoading] = useState(true); // Loading state
  const [cancellationNote, SetcancellationNote] = useState("");
  const [userdata, setUserdata] = useState(null);
  const [messageModalVisible, setMessageModalVisible] = useState(false); // State for message modal
  const [selectedMessages, setSelectedMessages] = useState([]); // State for selected order messages

  useEffect(() => {
    const user = JSON.parse(localStorage.getItem("user"));
    setUserdata(user);
    fetchOrdersAndCustomers();
  }, []);

  const fetchOrdersAndCustomers = async () => {
    try {
      setLoading(true); // Start loading
      const ordersRes = await axiosInstance.get(`${base_url}Order`, config());
      const customersRes = await axiosInstance.get(`${base_url}Users/customer`, config());

      setOrders(ordersRes.data);
      setCustomers(customersRes.data);
    } catch (error) {
      toast.error("Error fetching data");
    } finally {
      setLoading(false); // End loading
    }
  };

  const handleStatusChange = (value, order) => {
    setSelectedOrder(order);
    if (value === "Fulfilled") {
      setOpenFulfillModal(true); // Show fulfillment confirmation modal
    } else if (value === "Cancelled") {
      setOpenCancelModal(true); // Show cancellation modal
    } else {
      updateOrderStatus(order.id, value);
    }
  };

  const updateOrderStatus = async (orderId, status) => {
    try {
      await axiosInstance.patch(`${base_url}Order/${orderId}/status`, { status }, config());
      toast.success("Order status updated successfully!");
      fetchOrdersAndCustomers();
    } catch (error) {
      toast.error("Error updating order status");
    }
  };

  const confirmFulfillment = async () => {
    try {
      await axiosInstance.put(`${base_url}Order/${selectedOrder.id}/mark-delivered`, config());
      toast.success("Order marked as fulfilled successfully!");
      setOpenFulfillModal(false);
      fetchOrdersAndCustomers();
    } catch (error) {
      toast.error("Error marking order as fulfilled");
    }
  };

  const handleCancelOrder = async () => {
    try {
      await axiosInstance.put(
        `${base_url}Order/${selectedOrder.id}/cancel`,
        { cancellationNote }, // Wrap it in an object
        config() // Assuming config() already includes necessary headers
      );
      toast.success("Order cancelled successfully!");
      setOpenCancelModal(false);
      SetcancellationNote("");
      fetchOrdersAndCustomers();
    } catch (error) {
      toast.error("Error cancelling order");
    }
  };

  const handleMessageClick = (message) => {
    console.log(message);
    setSelectedMessages(message); // Set the messages for the selected order
    setMessageModalVisible(true); // Show the message modal
  };

  const getStatusStyle = (status) => {
    let bgColor, textColor;
    switch (status) {
      case "Pending":
        bgColor = "#FFE4B5"; // Light orange background
        textColor = "#FFA500"; // Orange text
        break;
      case "PartiallyFulfilled":
        bgColor = "#ADD8E6"; // Light blue background
        textColor = "#1E90FF"; // Blue text
        break;
      case "Fulfilled":
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

  const data = orders.map((order, index) => {
    const customer = customers.find((c) => c.id === order.userId) || {};

    // Define the available statuses
    const availableStatuses = ["Fulfilled", "Cancelled"];
    
    // Filter out the current order status from the dropdown options
    const statusOptions = availableStatuses.filter(status => status !== order.status);

    return {
      key: index + 1,
      orderid: order.orderId,
      cname: customer.username || "Unknown",
      address: `${order.shippingAddress.street}, ${order.shippingAddress.city}, ${order.shippingAddress.zip}`,
      pnumber: customer.phoneNumber || "Unknown",
      messageCount: order.messages ? order.messages.length : 0, // Safely access messages
      message:order.messages,
      date: new Date(order.createdAt).toLocaleDateString(),
      ostatus: order.status,
      oitem: (
        userdata.role === "CSR" ? (
          <Link
            to={`/csr/main-orders/${order.id}/${order.orderId}`}
            style={{
              backgroundColor: "#000429",
              padding: "5px 10px",
              borderRadius: "4px",
              color: "#ffffff",
              textDecoration: "none",
            }}
          >
            View
          </Link>
        ) : (
          <Link
            to={`/administrator/main-orders/${order.id}/${order.orderId}`}
            style={{
              backgroundColor: "#000429",
              padding: "5px 10px",
              borderRadius: "4px",
              color: "#ffffff",
              textDecoration: "none",
            }}
          >
            View
          </Link>
        )
      ),
      changeStatus: (
        <Select
          defaultValue={order.status}
          onChange={(value) => handleStatusChange(value, order)}
          style={{ width: 120 }}
        >
          {statusOptions.map(status => (
            <Option key={status} value={status}>
              {status}
            </Option>
          ))}
        </Select>
      ),
    };
  });

  return (
    <div>
      <h3 className="mb-4 title">All Customer Orders</h3>
      {loading ? ( // Show loading spinner while fetching data
        <div style={{ textAlign: 'center', marginTop: '50px' }}>
          <Spin size="large" />
        </div>
      ) : (
        <div>
          <Table columns={columns} dataSource={data} />
        </div>
      )}
      {/* Fulfillment Confirmation Modal */}
      <Modal
        title="Confirm Fulfillment"
        visible={openFulfillModal}
        onCancel={() => setOpenFulfillModal(false)}
        footer={[
          <Button key="back" onClick={() => setOpenFulfillModal(false)}>
            Cancel
          </Button>,
          <Button key="submit" type="primary" onClick={confirmFulfillment}>
            Confirm
          </Button>,
        ]}
      >
        <p>Are you sure you want to mark this order as fulfilled?</p>
      </Modal>

      {/* Cancellation Modal */}
      <Modal
        title="Cancel Order"
        visible={openCancelModal}
        onCancel={() => setOpenCancelModal(false)}
        footer={[
          <Button key="back" onClick={() => setOpenCancelModal(false)}>
            Cancel
          </Button>,
          <Button key="submit" type="primary" onClick={handleCancelOrder}>
            Submit
          </Button>,
        ]}
      >
        <p>Please provide a reason for cancellation:</p>
        <Input.TextArea
          value={cancellationNote}
          onChange={(e) => SetcancellationNote(e.target.value)}
          rows={4}
          placeholder="Enter cancellation note"
        />
      </Modal>

      {/* Message Modal */}
      <Modal
        title="Order Messages"   
        visible={messageModalVisible}
        onCancel={() => setMessageModalVisible(false)}
        footer={[
          <Button key="back" onClick={() => setMessageModalVisible(false)}>
            Close
          </Button>,
        ]}
      >
         {selectedMessages.length === 0 ? (
    <p>No messages available for this order.</p>
  ) : (
    <div style={{ maxHeight: '300px', overflowY: 'auto' }}>
      {selectedMessages.map((message, index) => (
        <div key={index} style={{ 
          padding: '10px', 
          marginBottom: '10px', 
          borderRadius: '5px', 
          backgroundColor: '#f1f1f1', 
          border: '1px solid #ccc' 
        }}>
          {message}
        </div>
      ))}
    </div>
  )}
      </Modal>
    </div>
  );
};

export default MainOrderList;
