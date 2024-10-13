import React, { useEffect, useState } from "react";
import { Table, Switch } from "antd";
import axios from "axios";
import { toast } from "react-toastify";
import { base_url } from "../utils/base_url";
import { axiosInstance, config } from "../utils/axiosConfig";


const columns = [
  {
    title: "Number",
    dataIndex: "key",
  },
  {
    title: "Name",
    dataIndex: "name",
    sorter: (a, b) => a.name.length - b.name.length,
  },
  {
    title: "Email",
    dataIndex: "email",
    sorter: (a, b) => a.email.length - b.email.length,
  },
  {
    title: "Mobile",
    dataIndex: "mobile",
  },
  {
    title: "Created Date",
    dataIndex: "date",
  },
  {
    title: "Activation/Deactivation",
    dataIndex: "isActive",
    render: (isActive, record) => (
      <Switch
        checked={isActive}
        //onChange={() => toggleUserStatus(record.id, !isActive)}
      />
    ),
  },
];

const Customers = () => {
  const [customers, setCustomers] = useState([]);

  // Fetch users (customers)
  const fetchCustomers = async () => {
    try {
      const res = await axiosInstance.get(`${base_url}Users/customer`, config());
      setCustomers(res.data);
    } catch (error) {
      toast.error("Error fetching customers");
    }
  };

  // Toggle user activation status
  const toggleUserStatus = async (id, newStatus) => {
    try {
      const endpoint = newStatus
        ? `${base_url}users/${id}/activate`
        : `${base_url}users/${id}/deactivate`;

      await axiosInstance.patch(endpoint, null,config());
      toast.success(`User ${newStatus ? "activated" : "deactivated"} successfully`);
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
        <Table columns={columns} dataSource={data} />
      </div>
    </div>
  );
};

export default Customers;
