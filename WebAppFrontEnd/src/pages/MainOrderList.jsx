import React, { useEffect, useState } from "react";
import { Table, Switch, Select, Modal, Input, Button } from "antd";
import { Link } from "react-router-dom";
import axios from "axios";
import { toast } from "react-toastify";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";

const { Option } = Select;


const MainOrderList = () => {
  const columns = [
    {
      title: "Number",
      dataIndex: "key",
    },
    {
      title: "OrderID",
      dataIndex: "orderid",
      sorter: (a, b) => a.vname.length - b.vname.length,
    },
    {
      title: "Customer Name",
      dataIndex: "cname",
      sorter: (a, b) => a.email.length - b.email.length,
    },
    {
      title: "Delivery Address",
      dataIndex: "address",
      sorter: (a, b) => a.business.length - b.business.length,
    },
    {
      title: "Phone Number",
      dataIndex: "pnumber",
    },
    {
      title: "Ordered Date",
      dataIndex: "date",
    },
    {
      title: "Order Status",
      dataIndex: "ostatus",
    },
    {
      title: "Order Items",
      dataIndex: "oitem",
    },
    {
      title: "Change Status",
      dataIndex: "changeStatus",
    },
  ];

  const [orders, setOrders] = useState([]);
  const [customers, setCustomers] = useState({});
  const [open, setOpen] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [cancellationNote, setCancellationNote] = useState("");

  useEffect(() => {
    fetchOrdersAndCustomers();
  }, []);

  const fetchOrdersAndCustomers = async () => {
    try {
      const ordersRes = await axiosInstance.get(`${base_url}Order`, config());

      const customersRes = await axiosInstance.get(`${base_url}Users/customer`,config());

      setOrders(ordersRes.data);
      setCustomers(customersRes.data);
    } catch (error) {
      toast.error("Error fetching data");
    }
  };

  const handleStatusChange = (value, order) => {
    if (value === "Cancelled") {
      setSelectedOrder(order);
      setOpen(true);
    } else {
      updateOrderStatus(order.id, value);
    }
  };

  const updateOrderStatus = async (orderId, status) => {
    try {
      await axiosInstance.patch(`${base_url}Order/${orderId}/status`, status,config());
      toast.success("Order status updated successfully!");
      fetchOrdersAndCustomers();
    } catch (error) {
      toast.error("Error updating order status");
    }
  };

  const handleCancelOrder = async () => {
    try {
      await axiosInstance.patch(`${base_url}Order/${selectedOrder.id}/cancel`, { note: cancellationNote },config());
      toast.success("Order cancelled successfully!");
      setOpen(false);
      setCancellationNote("");
      fetchOrdersAndCustomers();
    } catch (error) {
      toast.error("Error cancelling order");
    }
  };

  const data = orders.map((order, index) => {
    const customer = customers.find((c) => c.id === order.userId) || {};

    return {
      key: index + 1,
      orderid: order.id,
      cname: customer.username || "Unknown",
      address: `${order.shippingAddress.street}, ${order.shippingAddress.city}, ${order.shippingAddress.zip}`,
      pnumber: customer.phoneNumber || "Unknown",
      tamount: order.totalAmount + ".00",
      date: new Date(order.createdAt).toLocaleDateString(),
      ostatus: order.status,
      oitem: (
        <Link
          to={`/administrator/main-orders/${order.id}`}
          style={{
            backgroundColor: "#f0f0f0",
            padding: "5px 10px",
            borderRadius: "4px",
            color: "#007bff",
            textDecoration: "none",
          }}
        >
          View
        </Link>
      ),
      changeStatus: (
        <Select
          defaultValue={order.status}
          onChange={(value) => handleStatusChange(value, order)}
          style={{ width: 120 }}
        >
          <Option value="Pending">Pending</Option>
          <Option value="PartiallyFulfilled">Partially Fulfilled</Option>
          <Option value="Fulfilled">Fulfilled</Option>
          <Option value="Cancelled">Cancelled</Option>
        </Select>
      ),
    };
  });

  return (
    <div>
      <h3 className="mb-4 title">All Orders</h3>
      <div>
        <Table columns={columns} dataSource={data} />
      </div>
      <Modal
        title="Cancel Order"
        visible={open}
        onCancel={() => setOpen(false)}
        footer={[
          <Button key="back" onClick={() => setOpen(false)}>
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
          onChange={(e) => setCancellationNote(e.target.value)}
          rows={4}
          placeholder="Enter cancellation note"
        />
      </Modal>
    </div>
  );
};

export default MainOrderList;
