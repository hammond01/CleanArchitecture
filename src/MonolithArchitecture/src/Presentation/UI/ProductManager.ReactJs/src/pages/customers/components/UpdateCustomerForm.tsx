import { ModalForm, ProFormText } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message, Form } from 'antd';
import React, { useEffect } from 'react';
import { customerApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void; customerId?: string }

const UpdateCustomerForm: React.FC<Props> = ({ visible, onCancel, onSuccess, customerId }) => {
  const [messageApi, contextHolder] = message.useMessage();
  const [form] = Form.useForm();

  useEffect(() => {
    let mounted = true;
    if (visible) {
      if (customerId) {
        customerApi.getById(String(customerId)).then(res => { if (!mounted) return; if (res && res.statusCode === 200) form.setFieldsValue(res.data); else messageApi.error('Failed to load'); }).catch(() => messageApi.error('Failed to load'));
      } else { form.resetFields(); }
    }
    return () => { mounted = false; };
  }, [visible, customerId]);

  return (
    <>
      {contextHolder}
      <ModalForm form={form} title={<FormattedMessage id="pages.customer.edit" defaultMessage="Edit Customer" />} width={420} visible={visible} onVisibleChange={(v) => !v && onCancel()} onFinish={async (vals) => {
        try { let res: any = null; if (customerId) res = await customerApi.update(String(customerId), vals as any); else res = await customerApi.create(vals as any); if (res && (res.statusCode === 200 || res.statusCode === 201)) { message.success('Saved'); onSuccess(); return true; } message.error(res?.message || 'Failed'); return false; } catch { message.error('Failed'); return false; }
      }}>
        <ProFormText name="companyName" label="Company" rules={[{ required: true }]} />
        <ProFormText name="contactName" label="Contact" />
        <ProFormText name="phone" label="Phone" />
        <ProFormText name="country" label="Country" />
      </ModalForm>
    </>
  );
};

export default UpdateCustomerForm;
