import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { orderApi } from '@/services/api';
import type { Order } from '@/services/api';
import CreateOrderForm from './components/CreateOrderForm';
import UpdateOrderForm from './components/UpdateOrderForm';

const OrderList: React.FC = () => {
  const actionRef = useRef<ActionType>(null);
  const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
  const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Order>();
  const [selectedRowsState, setSelectedRows] = useState<Order[]>([]);

  const columns: ProColumns<Order>[] = [
    { title: 'ID', dataIndex: 'id' },
    { title: 'Customer', dataIndex: 'customerId' },
    { title: 'Employee', dataIndex: 'employeeId', responsive: ['md'] },
    { title: 'Order Date', dataIndex: 'orderDate', responsive: ['lg'] },
    { title: 'Shipped Date', dataIndex: 'shippedDate' },
    { title: 'Ship Country', dataIndex: 'shipCountry' },
    {
      title: 'Actions', dataIndex: 'option', valueType: 'option', render: (_, record) => [
        <a key="edit" onClick={() => { setCurrentRow(record); setUpdateModalVisible(true); }}>Edit</a>,
        <Popconfirm key="delete" title="Are you sure to delete this order?" onConfirm={async () => { try { await orderApi.delete(record.id); message.success('Order deleted'); actionRef.current?.reload(); } catch { message.error('Failed to delete order'); } }} okText="Yes" cancelText="No"><a key="delete" style={{ color: 'red' }}>Delete</a></Popconfirm>
      ]
    }
  ];

  return (
    <PageContainer>
      <ProTable<Order>
        headerTitle={<FormattedMessage id="pages.order.title" defaultMessage="Order Management" />}
        actionRef={actionRef}
        rowKey="id"
        request={async () => { try { const r = await orderApi.getAll(); if (r.statusCode === 200) return { data: r.data || [], success: true, total: (r.data || []).length }; return { data: [], success: false, total: 0 }; } catch { return { data: [], success: false, total: 0 }; } }}
        columns={columns}
        toolBarRender={() => [<Button key="new" type="primary" onClick={() => setCreateModalVisible(true)}>New</Button>]}
        rowSelection={{ onChange: (_, rows) => setSelectedRows(rows) }}
      />

      <CreateOrderForm visible={createModalVisible} onCancel={() => setCreateModalVisible(false)} onSuccess={() => { setCreateModalVisible(false); actionRef.current?.reload(); }} />

      <UpdateOrderForm visible={updateModalVisible} orderId={currentRow?.id} onCancel={() => setUpdateModalVisible(false)} onSuccess={() => { setUpdateModalVisible(false); setCurrentRow(undefined); actionRef.current?.reload(); }} />

      {selectedRowsState?.length > 0 && (
        <div style={{ marginTop: 16 }}>
          <Button onClick={async () => { try { await Promise.all(selectedRowsState.map(o => orderApi.delete(o.id))); message.success('Orders deleted'); actionRef.current?.reload(); setSelectedRows([]); } catch { message.error('Failed to delete'); } }}>Batch delete</Button>
        </div>
      )}
    </PageContainer>
  );
};

export default OrderList;
