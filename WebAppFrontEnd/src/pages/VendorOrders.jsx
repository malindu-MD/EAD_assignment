import React, { useEffect, useState } from "react";
import { Table, Spin } from "antd"; // Import Spin
import { Link } from "react-router-dom";
import { toast } from "react-toastify";
import { axiosInstance, config } from "../utils/axiosConfig";
import { base_url } from "../utils/base_url";
import CustomModal from "../components/CustomModal";

const VendorOrders = () => {
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
      render: (status) => {
        let bgColor = "";
        let textColor = "";
        switch (status) {
          case "Pending":
            bgColor = "#FFE4B5"; // Light orange background
            textColor = "#FFA500"; // Orange text
            break;
          case "PartiallyFulfilled":
            bgColor = "#FFD700"; // Gold background
            textColor = "#FF8C00"; // Dark orange text
            break;
          case "Fulfilled":
            bgColor = "#90EE90"; // Light green background
            textColor = "#32CD39"; // Green text
            break;
          case "Cancelled":
            bgColor = "#FFC0CB"; // Light pink background
            textColor = "#FF0000"; // Red text
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
              borderRadius: "12px", // Rounded corners
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
      title: "Order Items",
      align: "center",
      dataIndex: "oitem",
    },
  ];

  const [orders, setOrders] = useState([]);
  const [customers, setCustomers] = useState({});
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
  }, []);

  // Fetch all products
  const fetchOrdersAndCustomers = async () => {
    try {
      setLoading(true); // Set loading to true when fetching starts
      const ordersRes = await axiosInstance.get(`${base_url}Order/vendor`, config());
      const customersRes = await axiosInstance.get(`${base_url}Users/customer`, config());

      setOrders(ordersRes.data);
      setCustomers(customersRes.data);
    } catch (error) {
      toast.error("Error fetching data");
    } finally {
      setLoading(false); // Set loading to false after fetching
    }
  };

  const data = orders.map((order, index) => {
    const customer = customers.find((c) => c.id === order.userId) || {};

    return {
      key: index + 1,
      orderid: order.orderId,
      cname: customer.username || "Unknown",
      address: `${order.shippingAddress.street}, ${order.shippingAddress.city}, ${order.shippingAddress.zip}`,
      pnumber: customer.phoneNumber || "Unknown",
      date: new Date(order.createdAt).toLocaleDateString(),
      ostatus: order.status,
      oitem: (
        <Link
          to={`/vendor/vendor-orders/${order.id}/${order.orderId}`}
          style={{
            backgroundColor: "#0c0954", // Set your desired background color
            padding: "5px 10px", // Add some padding for better appearance
            borderRadius: "4px", // Optional: Make the edges rounded
            color: "#ffffff", // Adjust text color for contrast
            textDecoration: "none", // Remove underline
          }}
        >
          View Order Items
        </Link>
      ),
    };
  });

  return (
    <div>
      <h3 className="mb-4 title">Vendor Order List</h3>
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

export default VendorOrders;
