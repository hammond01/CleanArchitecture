import { ModalForm, ProFormText, ProFormDatePicker } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message } from 'antd';
import React from 'react';
import { orderApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void }

const CreateOrderForm: React.FC<Props> = ({ visible, onCancel, onSuccess }) => {
  const [messageApi, contextHolder] = message.useMessage();
  return (
    <>
      {contextHolder}
      <ModalForm title={<FormattedMessage id="pages.order.new" defaultMessage="New Order" />} width={520} visible={visible} onVisibleChange={(v) => !v && onCancel()} onFinish={async (vals) => {
        try { const res = await orderApi.create(vals); if (res && res.statusCode === 201) { messageApi.success('Order created'); onSuccess(); return true; } messageApi.error(res?.message || 'Failed'); return false; } catch { messageApi.error('Failed'); return false; }
      }}>
        <ProFormText name="customerId" label="Customer ID" rules={[{ required: true }]} />
        <ProFormText name="employeeId" label="Employee ID" />
        <ProFormDatePicker name="orderDate" label="Order Date" />
        <ProFormDatePicker name="shippedDate" label="Shipped Date" />
        <ProFormText name="shipCountry" label="Ship Country" />
        <ProFormText name="shipName" label="Ship Name" />
        <ProFormText name="shipAddress" label="Ship Address" />
      </ModalForm>
    </>
  );
};

export default CreateOrderForm;
