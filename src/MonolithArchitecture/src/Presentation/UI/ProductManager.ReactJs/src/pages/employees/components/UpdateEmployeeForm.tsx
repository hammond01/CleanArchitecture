import { ModalForm, ProFormText, ProFormDatePicker } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message, Form } from 'antd';
import React, { useEffect } from 'react';
import { employeeApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void; employeeId?: string }

const UpdateEmployeeForm: React.FC<Props> = ({ visible, onCancel, onSuccess, employeeId }) => {
  const [messageApi, contextHolder] = message.useMessage();
  const [form] = Form.useForm();

  useEffect(() => {
    let mounted = true;
    if (visible) {
      if (employeeId) {
        employeeApi.getById(String(employeeId)).then(res => { if (!mounted) return; if (res && res.statusCode === 200) form.setFieldsValue(res.data); else messageApi.error('Failed to load'); }).catch(() => messageApi.error('Failed to load'));
      } else form.resetFields();
    }
    return () => { mounted = false; };
  }, [visible, employeeId]);

  return (
    <>
      {contextHolder}
      <ModalForm form={form} title={<FormattedMessage id="pages.employee.edit" defaultMessage="Edit Employee" />} width={480} visible={visible} onVisibleChange={(v) => !v && onCancel()} onFinish={async (vals) => {
        try { let res: any = null; if (employeeId) res = await employeeApi.update(String(employeeId), vals as any); else res = await employeeApi.create(vals as any); if (res && (res.statusCode === 200 || res.statusCode === 201)) { message.success('Saved'); onSuccess(); return true; } message.error(res?.message || 'Failed'); return false; } catch { message.error('Failed'); return false; }
      }}>
        <ProFormText name="firstName" label="First Name" rules={[{ required: true }]} />
        <ProFormText name="lastName" label="Last Name" />
        <ProFormText name="title" label="Title" />
        <ProFormText name="city" label="City" />
        <ProFormDatePicker name="hireDate" label="Hire Date" />
      </ModalForm>
    </>
  );
};

export default UpdateEmployeeForm;
