import { ModalForm, ProFormText, ProFormDatePicker } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message, Form } from 'antd';
import React, { useEffect } from 'react';
import { orderApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void; orderId?: string }

const UpdateOrderForm: React.FC<Props> = ({ visible, onCancel, onSuccess, orderId }) => {
  const [messageApi, contextHolder] = message.useMessage();
  const [form] = Form.useForm();

  useEffect(() => {
    let mounted = true;
    if (visible) {
      if (orderId) {
        orderApi.getById(String(orderId)).then(res => { if (!mounted) return; if (res && res.statusCode === 200) form.setFieldsValue(res.data); else messageApi.error('Failed to load'); }).catch(() => messageApi.error('Failed to load'));
      } else form.resetFields();
    }
    return () => { mounted = false; };
  }, [visible, orderId]);

  return (
    <>
      {contextHolder}
      <ModalForm form={form} title={<FormattedMessage id="pages.order.edit" defaultMessage="Edit Order" />} width={520} visible={visible} onVisibleChange={(v) => !v && onCancel()} onFinish={async (vals) => {
        try { let res: any = null; if (orderId) res = await orderApi.update(String(orderId), vals); else res = await orderApi.create(vals); if (res && (res.statusCode === 200 || res.statusCode === 201)) { message.success('Saved'); onSuccess(); return true; } message.error(res?.message || 'Failed'); return false; } catch { message.error('Failed'); return false; }
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

export default UpdateOrderForm;
