import React, { useEffect, useState } from "react";
import { Table, Switch, Spin } from "antd"; // Import Spin
import axios from "axios";
import { toast } from "react-toastify";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";


const Customers = () => {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true); // Loading state

  // Fetch users (customers)
  const fetchCustomers = async () => {
    try {
      setLoading(true); // Start loading
      const res = await axiosInstance.get(`${base_url}Users/customer`, config());
      setCustomers(res.data);
    } catch (error) {
      toast.error("Error fetching customers");
    } finally {
      setLoading(false); // End loading
    }
  };

  // Toggle user activation status
  const toggleUserStatus = async (id, newStatus) => {
    try {
      const endpoint = newStatus
        ? `${base_url}users/reactivate/${id}`
        : `${base_url}users/deactivate/${id}`;

      await axiosInstance.put(endpoint, config());
      toast.success(`User ${newStatus ? "Account activated" : "Account deactivated"} successfully`);
      fetchCustomers(); // Refetch the customers after status change
    } catch (error) {
      toast.error("Error updating user status");
    }
  };

  useEffect(() => {
    fetchCustomers();
  }, []);

  const data = customers.map((customer, index) => ({
    key: index + 1,
    name: customer.username,
    email: customer.email,
    mobile: customer.phoneNumber,
    date: new Date(customer.createdAt).toLocaleDateString(),
    isActive: customer.isActive,
    id: customer.id,
  }));

  return (
    <div>
      <h3 className="mb-4 title">Customers</h3>
      <div>
        {loading ? ( // Show spinner while loading
          <div style={{ textAlign: 'center', marginTop: '50px' }}>
            <Spin size="large" />
          </div>
        ) : (
          <Table columns={[
            {
              title: "Number",
              dataIndex: "key",
              align: "center",
            },
            {
              title: "Customer Name",
              dataIndex: "name",
              align: "center",
              sorter: (a, b) => a.name.length - b.name.length,
            },
            {
              title: "Email",
              dataIndex: "email",
              align: "center",
              sorter: (a, b) => a.email.length - b.email.length,
            },
            {
              title: "Phone Number",
              dataIndex: "mobile",
              align: "center",
            },
            {
              title: "Account Created Date",
              dataIndex: "date",
              align: "center",
              sorter: (a, b) => new Date(a.date) - new Date(b.date), // Sorting function
              render: (date) => new Date(date).toLocaleDateString(), // Format date for display
            },
            {
              title: "Account Activation/Deactivation",
              dataIndex: "isActive",
              align: "center",
              render: (isActive, record) => (
                <Switch
                  checked={isActive}
                  onChange={() => toggleUserStatus(record.id, !isActive)}
                />
              ),
            },
          ]} dataSource={data} />
        )}
      </div>
    </div>
  );
};

export default Customers;
