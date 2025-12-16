import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { customerApi } from '@/services/api';
import type { Customer } from '@/services/api';
import CreateCustomerForm from './components/CreateCustomerForm';
import UpdateCustomerForm from './components/UpdateCustomerForm';

const CustomerList: React.FC = () => {
  const actionRef = useRef<ActionType>(null);
  const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);
  const [updateModalVisible, setUpdateModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Customer>();
  const [selectedRowsState, setSelectedRows] = useState<Customer[]>([]);

  const columns: ProColumns<Customer>[] = [
    { title: 'ID', dataIndex: 'id' },
    { title: 'Company', dataIndex: 'companyName' },
    { title: 'Contact', dataIndex: 'contactName' },
    { title: 'Phone', dataIndex: 'phone' },
    { title: 'Country', dataIndex: 'country' },
    {
      title: 'Actions', dataIndex: 'option', valueType: 'option', render: (_, record) => [
        <a key="edit" onClick={() => { setCurrentRow(record); setUpdateModalVisible(true); }}>Edit</a>,
        <Popconfirm key="delete" title="Are you sure to delete this customer?" onConfirm={async () => { try { await customerApi.delete(record.id); message.success('Customer deleted'); actionRef.current?.reload(); } catch { message.error('Failed to delete customer'); } }} okText="Yes" cancelText="No"><a key="delete" style={{ color: 'red' }}>Delete</a></Popconfirm>
      ]
    }
  ];

  return (
    <PageContainer>
      <ProTable<Customer>
        headerTitle={<FormattedMessage id="pages.customer.title" defaultMessage="Customer Management" />}
        actionRef={actionRef}
        rowKey="id"
        request={async () => {
          try { const res = await customerApi.getAll(); if (res.statusCode === 200) return { data: res.data || [], success: true, total: (res.data || []).length }; return { data: [], success: false, total: 0 }; } catch { return { data: [], success: false, total: 0 }; }
        }}
        columns={columns}
        toolBarRender={() => [<Button key="new" type="primary" onClick={() => setCreateModalVisible(true)}>New</Button>]}
        rowSelection={{ onChange: (_, rows) => setSelectedRows(rows) }}
      />

      <CreateCustomerForm visible={createModalVisible} onCancel={() => setCreateModalVisible(false)} onSuccess={() => { setCreateModalVisible(false); actionRef.current?.reload(); }} />

      <UpdateCustomerForm visible={updateModalVisible} customerId={currentRow?.id} onCancel={() => setUpdateModalVisible(false)} onSuccess={() => { setUpdateModalVisible(false); setCurrentRow(undefined); actionRef.current?.reload(); }} />

      {selectedRowsState?.length > 0 && (
        <div style={{ marginTop: 16 }}>
          <Button onClick={async () => { try { await Promise.all(selectedRowsState.map(c => customerApi.delete(c.id))); message.success('Customers deleted'); actionRef.current?.reload(); setSelectedRows([]); } catch { message.error('Failed to delete'); } }}>Batch delete</Button>
        </div>
      )}
    </PageContainer>
  );
};

export default CustomerList;
