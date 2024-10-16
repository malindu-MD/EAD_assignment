import React, { useEffect, useState } from "react";
import { Table, Switch, Spin, Modal } from "antd"; // Import Spin
import { Link } from "react-router-dom";
import { BiEdit } from "react-icons/bi";
import { TiDeleteOutline } from "react-icons/ti";
import axios from "axios";
import { toast } from "react-toastify";
import { FaStar } from "react-icons/fa"; // Import the star icon
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
      title: " Name",
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
      title: "Phone",
      dataIndex: "phone",
      align: "center",
      
    },
    {
      title: "Business Name",
      dataIndex: "business",
      align: "center",
      sorter: (a, b) => a.business.length - b.business.length,
    },
    {
      title: (
        <span>
          Average Rating[5] <FaStar style={{ color: 'gold', size: 50 }} />
        </span>
      ),
      dataIndex: "arating",
      sorter: (a, b) => a.arating - b.arating,
      align: "center",
    },
    {
      title: "Customer Feedback",
      dataIndex: "comment",
      align: "center",
    },
    {
      title: "Date",
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
  const [loading, setLoading] = useState(true); // Loading state
  const [open, setOpen] = useState(false);
  const [selectedVendorComments, setSelectedVendorComments] = useState([]);
  const [deletingVendorId, setDeletingVendorId] = useState(""); // State to track vendor ID to delete

  const showModal = (vendorComments) => {
    setSelectedVendorComments(vendorComments);
    setOpen(true);
  };

  const hideModal = () => {
    setOpen(false);
  };

  useEffect(() => {
    fetchVendor();
  }, []);

  // Fetch all vendors
  const fetchVendor = async () => {
    try {
      const res = await axiosInstance.get(`${base_url}Vendor`, config());
      setVendors(res.data);
    } catch (error) {
      toast.error("Error fetching vendors");
    } finally {
      setLoading(false); // Set loading to false after fetching
    }
  };

  const handleDeleteVendor = async () => {
    try {
      console.log(deletingVendorId);
      await axiosInstance.delete(`${base_url}Vendor/${deletingVendorId}`, config());
    
      setOpen(false);
      toast.success("Vendor deleted successfully");
      fetchVendor();
    } catch (error) {
      toast.error("Error deleting vendor");
    }
  };

  const data = vendors.map((vendor, index) => ({
    key: index + 1,
    vname: vendor.username,
    email: vendor.email,
    phone:vendor.phoneNumber,
    business: vendor.businessName,
    arating: vendor.averageRating,
    comment: (
      <button className="btn btn-primary" onClick={() => showModal(vendor.comments)}>
        View
      </button>
    ),
    date: new Date(vendor.createdAt).toLocaleDateString(),
    action: (
      <>
        <Link to={`/administrator/vendor/${vendor.vendorId}`} className="text-success fs-3">
          <BiEdit />
        </Link>
        <button
          className="ms-3 text-danger fs-3 bg-transparent border-0"
          onClick={() => {
            setDeletingVendorId(vendor.vendorId);
            Modal.confirm({
              title: 'Are you sure you want to delete this vendor?',
              onOk: async () => {
                try {
                  // Call the delete function directly with the vendorId
                  await axiosInstance.delete(`${base_url}Vendor/${vendor.vendorId}`, config());
                  // Update the vendors state to remove the deleted vendor
                  setVendors(vendors.filter((v) => v.vendorId !== vendor.vendorId));
                  toast.success("Vendor deleted successfully");
                } catch (error) {
                  toast.error("Error deleting vendor");
                }
              },
            });
          }}
        >
          <TiDeleteOutline />
        </button>
      </>
    ),
  }));

  return (
    <div>
      <h3 className="mb-4 title">Vendor List</h3>
      {loading ? (
        <div style={{ textAlign: 'center', marginTop: '50px' }}>
          <Spin size="large" />
        </div>
      ) : (
        <div>
          <Table columns={columns} dataSource={data} />
        </div>
      )}
      <Modal
        title="Vendor Comments"
        visible={open}
        onCancel={hideModal}
        footer={[
          <button key="back" onClick={hideModal}>
            Close
          </button>,
        ]}
      >
        {selectedVendorComments.length === 0 ? (
          <p>No comments available.</p>
        ) : (
          <ul>
            {selectedVendorComments.map((comment, index) => (
              <li key={index}>
                <strong>{comment.displayName}:</strong> {comment.comment} (Rating: {comment.rating})
              </li>
            ))}
          </ul>
        )}
      </Modal>
    </div>
  );
};

export default VendorList;
