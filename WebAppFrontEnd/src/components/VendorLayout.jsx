import React, { useState } from "react";
import { MenuFoldOutlined, MenuUnfoldOutlined } from "@ant-design/icons";
import {
  AiOutlineDashboard,
  AiOutlineAppstoreAdd,
  AiOutlineUnorderedList,
} from "react-icons/ai";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { BiCategoryAlt } from "react-icons/bi";
import { FaClipboardList } from "react-icons/fa";
import { IoNotificationsSharp } from "react-icons/io5";
import { RiCouponLine } from "react-icons/ri";
import { CiLogout } from "react-icons/ci";
import { Layout, Menu, theme, Modal } from "antd";
import { Link, useNavigate } from "react-router-dom";
import { Outlet } from "react-router-dom";
import { useSelector } from "react-redux";
import { CheckCircleOutlined, ExclamationCircleOutlined } from '@ant-design/icons';

const { Header, Sider, Content } = Layout;

const VendorLayout = () => {
  const [collapsed, setCollapsed] = useState(false);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [notifications, setNotifications] = useState([
    { id: 1, message: "You have a new order from John Doe.", read: false },
    { id: 2, message: "Your order #1234 has been shipped.", read: false },
    { id: 3, message: "New feedback received from Jane Smith.", read: false },
  ]);

  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const navigate = useNavigate();
  const authState = useSelector((state) => state?.auth?.user);

  const showModal = () => {
    setIsModalVisible(true);
  };

  const handleOk = () => {
    setIsModalVisible(false);
    markAllAsRead();
  };

  const handleCancel = () => {
    setIsModalVisible(false);
  };

  const markAllAsRead = () => {
    setNotifications((prevNotifications) =>
      prevNotifications.map((notification) => ({
        ...notification,
        read: true,
      }))
    );
  };

  return (
    <Layout onContextMenu={(e) => e.preventDefault()}>
      <Sider trigger={null} collapsible collapsed={collapsed}>
        <div className="logo" style={{ backgroundColor: "#488A99" }}>
          <h2 className="text-white fs-5 text-center py-3 mb-0">
            <span className="sm-logo">BM</span>
            <span className="lg-logo">Biz Corner</span>
          </h2>
        </div>
        <Menu
          theme="dark"
          mode="inline"
          defaultSelectedKeys={[""]}
          onClick={({ key }) => {
            if (key === "signout") {
              localStorage.clear();
              window.location.reload();
            } else {
              navigate(key);
            }
          }}
          items={[
            {
              key: "",
              icon: <AiOutlineDashboard className="fs-4" />,
              label: "Vendor Dashboard",
            },
            {
              key: "catalog",
              icon: <BiCategoryAlt className="fs-4" />,
              label: "Products",
              children: [
                {
                  key: "product",
                  icon: <AiOutlineAppstoreAdd className="fs-4" />,
                  label: "Add Product",
                },
                {
                  key: "product-list",
                  icon: <AiOutlineUnorderedList className="fs-4" />,
                  label: "Product List",
                },
              ],
            },
            {
              key: "vendor-orders",
              icon: <FaClipboardList className="fs-4" />,
              label: "Customer Orders",
            },
            {
              key: "marketing",
              icon: <RiCouponLine className="fs-4" />,
              label: "Customer Feedback",
            },
            {
              key: "signout",
              icon: <CiLogout className="fs-4" />,
              label: "Sign Out",
            },
          ]}
        />
      </Sider>
      <Layout className="site-layout">
        <Header
          className="d-flex justify-content-between ps-1 pe-5"
          style={{
            padding: 0,
            background: colorBgContainer,
          }}
        >
          {React.createElement(
            collapsed ? MenuUnfoldOutlined : MenuFoldOutlined,
            {
              className: "trigger",
              onClick: () => setCollapsed(!collapsed),
            }
          )}
          <div className="d-flex gap-4 align-items-center">
            <div className="position-relative" onClick={showModal} style={{ cursor: "pointer" }}>
              <IoNotificationsSharp className="fs-4" />
              <span className="badge bg-warning rounded-circle p-1 position-absolute">
                {notifications.filter((n) => !n.read).length}
              </span>
            </div>

            <div className="d-flex gap-3 align-items-center dropdown">
              <div>
                <img
                  width={32}
                  height={32}
                  src="https://media.licdn.com/dms/image/D5603AQF1oO-LOJxT_w/profile-displayphoto-shrink_800_800/0/1664684206755?e=1688601600&v=beta&t=2rtqrrVZMf-oqOjGIgZiGbx0qpGDjGZMn576ZBfQWb0"
                  alt="adminPic"
                />
              </div>
              <div
                role="button"
                id="dropdownMenuLink"
                data-bs-toggle="dropdown"
                aria-expanded="false"
              >
                <h5 className="mb-0">
                  {authState?.firstName + " " + authState?.lastName}
                </h5>
                <p className="mb-0">{authState?.email}</p>
              </div>
              <div className="dropdown-menu" aria-labelledby="dropdownMenuLink">
                <li>
                  <Link
                    className="dropdown-item py-1 mb-1"
                    style={{ height: "auto", lineHeight: "20px" }}
                    to="/"
                  >
                    View Profile
                  </Link>
                </li>
              </div>
            </div>
          </div>
        </Header>
        <Content
          style={{
            margin: "24px 16px",
            padding: 24,
            minHeight: 280,
            background: colorBgContainer,
          }}
        >
          <ToastContainer
            position="top-right"
            autoClose={250}
            hideProgressBar={false}
            newestOnTop={true}
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            draggable
            theme="light"
          />
          <Outlet />

          {/* Notification Modal */}
          <Modal
            title="Notifications"
            visible={isModalVisible}
            onOk={handleOk}
            onCancel={handleCancel}
          >
            {notifications.map((notification) => (
              <div key={notification.id} className="notification-item">
                {notification.read ? (
                  <CheckCircleOutlined style={{ color: "green", marginRight: 8 }} />
                ) : (
                  <ExclamationCircleOutlined style={{ color: "red", marginRight: 8 }} />
                )}
                <p
                  style={{
                    textDecoration: notification.read ? "line-through" : "none",
                    color: notification.read ? "gray" : "black",
                  }}
                >
                  {notification.message}
                </p>
                <hr />
              </div>
            ))}
          </Modal>
        </Content>
      </Layout>
    </Layout>
  );
};

export default VendorLayout;
